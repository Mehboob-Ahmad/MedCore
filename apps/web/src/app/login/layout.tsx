import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Sign In",
  description: "Securely log in to your MedicHp Doctor Portal, Patient Dashboard, or Administration Console.",
};

export default function LoginLayout({ children }: { children: React.ReactNode }) {
  return children;
}
