import type { ReactNode } from "react";

interface PageTitleProps {
  title: string;
  subtitle: string;
  actions?: ReactNode;
}

export function PageTitle({ title, subtitle, actions }: PageTitleProps) {
  return (
    <div className="page-title">
      <div>
        <h2>{title}</h2>
        <p>{subtitle}</p>
      </div>
      {actions ? <div className="page-title-actions">{actions}</div> : null}
    </div>
  );
}
