import { Navigate } from "react-router-dom";
import type { PropsWithChildren } from "react";
import type { StaffRole } from "../types/domain";

interface RoleGateProps extends PropsWithChildren {
  role: StaffRole;
  allowed: StaffRole;
}

export function RoleGate({ role, allowed, children }: RoleGateProps) {
  if (role !== allowed) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
