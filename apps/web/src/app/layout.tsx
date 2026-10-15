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
  title: {
    default: "MedicHp — Intelligent Healthcare Ecosystem",
    template: "%s | MedicHp",
  },
  description: "MedicHp is a digital healthcare platform connecting patients with verified doctors, featuring intelligent discovery, instant appointment booking, and secure medical records.",
  keywords: ["healthcare", "doctor appointment", "telemedicine", "digital clinic", "medical records", "online doctor", "MedicHp"],
  authors: [{ name: "MedicHp Health Systems" }],
  creator: "MedicHp",
  icons: {
    icon: [
      { url: "/favicon.ico", sizes: "32x32" },
      { url: "/favicon.png", sizes: "512x512", type: "image/png" },
      { url: "/favicon.svg", type: "image/svg+xml" },
    ],
    apple: [
      { url: "/apple-touch-icon.png", sizes: "180x180", type: "image/png" },
    ],
    shortcut: "/favicon.ico",
  },
  openGraph: {
    type: "website",
    locale: "en_US",
    url: "https://www.medichp.com",
    siteName: "MedicHp",
    title: "MedicHp — Digital Healthcare Ecosystem",
    description: "Intelligent doctor discovery, frictionless appointment booking, and complete consultation workflow.",
  },
  twitter: {
    card: "summary_large_image",
    title: "MedicHp — Digital Healthcare Ecosystem",
    description: "Intelligent doctor discovery, frictionless appointment booking, and complete consultation workflow.",
  },
  robots: {
    index: true,
    follow: true,
  },
};

export const viewport = {
  width: "device-width",
  initialScale: 1,
  maximumScale: 5,
  themeColor: "#026cb6",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html
      lang="en"
      className={`${inter.variable} h-full antialiased overflow-x-hidden`}
      suppressHydrationWarning
    >
      <body className="min-h-full flex flex-col bg-surface-50 dark:bg-slate-900 text-neutral-900 dark:text-gray-50 overflow-x-hidden selection:bg-sky-500 selection:text-white" suppressHydrationWarning>
        <AuthProvider>
          <GlobalHeader />
          <main className="flex-1 flex flex-col w-full overflow-x-hidden">{children}</main>
          <GlobalFooter />
        </AuthProvider>
      </body>
    </html>
  );
}

