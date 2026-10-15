import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Clinical & Patient Features",
  description: "Explore MedicHp features: specialized Doctor AI, real-time appointment scheduling, encrypted patient-doctor chat, and secure medical records.",
};

export default function FeaturesLayout({ children }: { children: React.ReactNode }) {
  return children;
}
