import { NavLink, Outlet } from "react-router-dom";
import type { StaffRole } from "../types/domain";
import type { AppLanguage } from "../i18n";
import { useI18n } from "../i18n";

interface AppShellProps {
  role: StaffRole;
  onRoleChange: (role: StaffRole) => void;
}

interface NavItem {
  to: string;
  label: string;
  managerOnly?: boolean;
}

export function AppShell({ role, onRoleChange }: AppShellProps) {
  const { language, setLanguage, text, roleLabel } = useI18n();

  const navItems: NavItem[] = [
    { to: "/", label: text.appShell.nav.dashboard },
    { to: "/reservations", label: text.appShell.nav.reservations },
    { to: "/customers", label: text.appShell.nav.customers },
    { to: "/rooms", label: text.appShell.nav.rooms },
    { to: "/reports", label: text.appShell.nav.reports, managerOnly: true },
  ];

  return (
    <div className="staff-app">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark" aria-hidden="true">
            X
          </span>
          <div>
            <p className="brand-title">Hotel Lakeview</p>
            <p className="brand-subtitle">{text.appShell.staffConsole}</p>
          </div>
        </div>

        <nav className="nav-menu" aria-label={text.appShell.mainNavigation}>
          {navItems
            .filter((item) => (item.managerOnly ? role === "Manager" : true))
            .map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                end={item.to === "/"}
                className={({ isActive }) =>
                  isActive ? "nav-item nav-item-active" : "nav-item"
                }
              >
                {item.label}
              </NavLink>
            ))}
        </nav>

        <div className="internal-note">{text.appShell.internalOnly}</div>
      </aside>

      <main className="main-area">
        <header className="topbar">
          <div>
            <p className="topbar-overline">Hotel Lakeview</p>
            <h1>{text.appShell.managementViewTitle}</h1>
          </div>

          <div className="topbar-controls">
            <label className="role-select">
              {text.appShell.languageLabel}
              <select
                value={language}
                onChange={(event) => setLanguage(event.target.value as AppLanguage)}
              >
                <option value="fi">Suomi</option>
                <option value="en">English</option>
              </select>
            </label>

            <label className="role-select">
              {text.appShell.roleLabel}
              <select
                value={role}
                onChange={(event) => onRoleChange(event.target.value as StaffRole)}
              >
                <option value="Receptionist">{roleLabel("Receptionist")}</option>
                <option value="Manager">{roleLabel("Manager")}</option>
              </select>
            </label>
          </div>
        </header>

        <section className="page-content">
          <Outlet />
        </section>
      </main>
    </div>
  );
}
