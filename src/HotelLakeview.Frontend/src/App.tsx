import { useEffect, useState } from "react";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AppShell } from "./components/AppShell";
import { RoleGate } from "./components/RoleGate";
import { I18nProvider } from "./i18n";
import { CustomersPage } from "./pages/CustomersPage";
import { DashboardPage } from "./pages/DashboardPage";
import { ReportsPage } from "./pages/ReportsPage";
import { ReservationsPage } from "./pages/ReservationsPage";
import { RoomsPage } from "./pages/RoomsPage";
import type { StaffRole } from "./types/domain";

const roleStorageKey = "hotel-lakeview-staff-role";

function App() {
  const [role, setRole] = useState<StaffRole>(() => {
    const storedValue = localStorage.getItem(roleStorageKey);
    return storedValue === "Manager" ? "Manager" : "Receptionist";
  });

  useEffect(() => {
    localStorage.setItem(roleStorageKey, role);
  }, [role]);

  return (
    <I18nProvider>
      <BrowserRouter>
        <Routes>
          <Route element={<AppShell role={role} onRoleChange={setRole} />}>
            <Route index element={<DashboardPage />} />
            <Route path="reservations" element={<ReservationsPage />} />
            <Route path="customers" element={<CustomersPage />} />
            <Route path="rooms" element={<RoomsPage />} />
            <Route
              path="reports"
              element={(
                <RoleGate role={role} allowed="Manager">
                  <ReportsPage />
                </RoleGate>
              )}
            />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </I18nProvider>
  );
}

export default App;
