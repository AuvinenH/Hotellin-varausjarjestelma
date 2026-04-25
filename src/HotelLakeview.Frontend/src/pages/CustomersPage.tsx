import { useMemo, useState } from "react";
import type { FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { hotelApi } from "../api/hotelApi";
import { MessagePanel } from "../components/MessagePanel";
import { PageTitle } from "../components/PageTitle";
import { useI18n } from "../i18n";
import type { Customer } from "../types/domain";
import { formatDate } from "../utils/format";
import { getErrorMessage } from "../utils/errors";

interface CustomerForm {
  fullName: string;
  email: string;
  phoneNumber: string;
  notes: string;
}

const initialForm: CustomerForm = {
  fullName: "",
  email: "",
  phoneNumber: "",
  notes: "",
};

export function CustomersPage() {
  const { text, language } = useI18n();
  const queryClient = useQueryClient();
  const [search, setSearch] = useState("");
  const [editingCustomer, setEditingCustomer] = useState<Customer | null>(null);
  const [form, setForm] = useState<CustomerForm>(initialForm);
  const [message, setMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const customersQuery = useQuery({
    queryKey: ["customers", search],
    queryFn: () => hotelApi.getCustomers(search || undefined),
  });

  const saveMutation = useMutation({
    mutationFn: async () => {
      if (editingCustomer) {
        await hotelApi.updateCustomer(editingCustomer.id, {
          fullName: form.fullName,
          email: form.email,
          phoneNumber: form.phoneNumber,
          notes: form.notes || null,
        });
        return text.customers.updatedSuccess;
      }

      await hotelApi.createCustomer({
        fullName: form.fullName,
        email: form.email,
        phoneNumber: form.phoneNumber,
        notes: form.notes || null,
      });

      return text.customers.createdSuccess;
    },
    onSuccess: async (successMessage) => {
      setMessage(successMessage);
      setErrorMessage(null);
      setEditingCustomer(null);
      setForm(initialForm);
      await queryClient.invalidateQueries({ queryKey: ["customers"] });
    },
    onError: (error) => {
      setErrorMessage(getErrorMessage(error, language));
      setMessage(null);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (customerId: string) => hotelApi.deleteCustomer(customerId),
    onSuccess: async () => {
      setMessage(text.customers.deletedSuccess);
      setErrorMessage(null);
      await queryClient.invalidateQueries({ queryKey: ["customers"] });
    },
    onError: (error) => {
      setErrorMessage(getErrorMessage(error, language));
      setMessage(null);
    },
  });

  const isLoading = customersQuery.isLoading || saveMutation.isPending || deleteMutation.isPending;

  const sortedCustomers = useMemo(() => {
    const locale = language === "fi" ? "fi" : "en";
    return [...(customersQuery.data ?? [])].sort((a, b) => a.fullName.localeCompare(b.fullName, locale));
  }, [customersQuery.data, language]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    saveMutation.mutate();
  }

  function startEditing(customer: Customer) {
    setEditingCustomer(customer);
    setForm({
      fullName: customer.fullName,
      email: customer.email,
      phoneNumber: customer.phoneNumber,
      notes: customer.notes ?? "",
    });
    setMessage(null);
    setErrorMessage(null);
  }

  function resetForm() {
    setEditingCustomer(null);
    setForm(initialForm);
    setMessage(null);
    setErrorMessage(null);
  }

  return (
    <div className="stack-large">
      <PageTitle
        title={text.customers.title}
        subtitle={text.customers.subtitle}
      />

      {message ? <MessagePanel tone="success" message={message} /> : null}
      {errorMessage ? <MessagePanel tone="error" message={errorMessage} /> : null}

      <section className="panel">
        <div className="inline-filters">
          <label className="grow">
            {text.customers.searchLabel}
            <input
              type="search"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder={text.customers.searchPlaceholder}
            />
          </label>
        </div>
      </section>

      <section className="panel split-panel">
        <form className="stack-medium" onSubmit={handleSubmit}>
          <h3>{editingCustomer ? text.customers.editCustomer : text.customers.addCustomer}</h3>

          <label>
            {text.customers.name}
            <input
              required
              value={form.fullName}
              onChange={(event) => setForm((previous) => ({ ...previous, fullName: event.target.value }))}
            />
          </label>

          <label>
            {text.customers.email}
            <input
              required
              type="email"
              value={form.email}
              onChange={(event) => setForm((previous) => ({ ...previous, email: event.target.value }))}
            />
          </label>

          <label>
            {text.customers.phone}
            <input
              required
              value={form.phoneNumber}
              onChange={(event) => setForm((previous) => ({ ...previous, phoneNumber: event.target.value }))}
            />
          </label>

          <label>
            {text.customers.notes}
            <textarea
              rows={4}
              value={form.notes}
              onChange={(event) => setForm((previous) => ({ ...previous, notes: event.target.value }))}
            />
          </label>

          <div className="button-row">
            <button className="btn-primary" type="submit" disabled={isLoading}>
              {editingCustomer ? text.common.saveChanges : text.customers.createButton}
            </button>
            {editingCustomer ? (
              <button className="btn-ghost" type="button" onClick={resetForm}>
                {text.common.cancelEdit}
              </button>
            ) : null}
          </div>
        </form>

        <div>
          <h3>{text.customers.listTitle}</h3>
          <table className="table">
            <thead>
              <tr>
                <th>{text.customers.name}</th>
                <th>{text.customers.email}</th>
                <th>{text.customers.phone}</th>
                <th>{text.customers.created}</th>
                <th>{text.common.actions}</th>
              </tr>
            </thead>
            <tbody>
              {customersQuery.isLoading ? (
                <tr>
                  <td colSpan={5}>{text.customers.loading}</td>
                </tr>
              ) : sortedCustomers.length === 0 ? (
                <tr>
                  <td colSpan={5}>{text.customers.noResults}</td>
                </tr>
              ) : (
                sortedCustomers.map((customer) => (
                  <tr key={customer.id}>
                    <td>
                      <div>{customer.fullName}</div>
                      {customer.notes ? <small className="muted-text">{customer.notes}</small> : null}
                    </td>
                    <td>{customer.email}</td>
                    <td>{customer.phoneNumber}</td>
                    <td>{formatDate(customer.createdAtUtc, language)}</td>
                    <td>
                      <div className="button-row compact">
                        <button
                          className="btn-ghost"
                          type="button"
                          onClick={() => startEditing(customer)}
                        >
                          {text.customers.editButton}
                        </button>
                        <button
                          className="btn-danger"
                          type="button"
                          onClick={() => deleteMutation.mutate(customer.id)}
                        >
                          {text.customers.deleteButton}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}
