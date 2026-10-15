import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Patient Registration",
  description: "Create your free MedicHp patient account to find certified doctors, book appointments, and store medical history securely.",
};

export default function PatientRegisterLayout({ children }: { children: React.ReactNode }) {
  return children;
}
