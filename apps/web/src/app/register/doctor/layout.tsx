import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Doctor Registration & Onboarding",
  description: "Join MedicHp as a verified healthcare professional. Manage your schedule, digital prescriptions, and patient records with specialized AI.",
};

export default function DoctorRegisterLayout({ children }: { children: React.ReactNode }) {
  return children;
}
