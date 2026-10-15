import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Request a Doctor Demo",
  description: "Schedule a personalized demonstration of MedicHp for your clinic, hospital, or private medical practice.",
};

export default function RequestDemoLayout({ children }: { children: React.ReactNode }) {
  return children;
}
