import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Empty State Showcase",
  description: "Preview zero-data responsive state displays across MedicHp portal interfaces.",
};

export default function EmptyStateLayout({ children }: { children: React.ReactNode }) {
  return children;
}
