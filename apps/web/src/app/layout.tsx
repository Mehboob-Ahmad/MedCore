import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import Link from "next/link";
import { Stethoscope } from "lucide-react";
import { AuthProvider } from "@/contexts/AuthContext";
import { GlobalHeader } from "@/components/GlobalHeader";
import { GlobalFooter } from "@/components/GlobalFooter";
const inter = Inter({
  variable: "--font-sans",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "MedicHp | Digital Healthcare Ecosystem",
  description: "Intelligent doctor discovery, frictionless appointment booking, and complete consultation workflow.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html
      lang="en"
      className={`${inter.variable} h-full antialiased`}
      suppressHydrationWarning
    >
      <body className="min-h-full flex flex-col bg-surface-50 dark:bg-slate-900 text-neutral-900 dark:text-gray-50" suppressHydrationWarning>
        <AuthProvider>
          <GlobalHeader />
          <main className="flex-1 flex flex-col">{children}</main>
          <GlobalFooter />
        </AuthProvider>
      </body>
    </html>
  );
}

