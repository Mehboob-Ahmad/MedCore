import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Verify Your Email",
  description: "Enter your 4-digit verification code to activate your MedicHp account.",
};

export default function VerifyEmailLayout({ children }: { children: React.ReactNode }) {
  return children;
}
