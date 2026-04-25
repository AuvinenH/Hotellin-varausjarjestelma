interface MessagePanelProps {
  tone: "error" | "success" | "info";
  message: string;
}

export function MessagePanel({ tone, message }: MessagePanelProps) {
  return <div className={`message-panel message-${tone}`}>{message}</div>;
}
