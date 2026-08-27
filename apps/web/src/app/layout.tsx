import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import Link from "next/link";
import { Stethoscope } from "lucide-react";

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
        <header className="sticky top-0 z-50 w-full border-b border-gray-200 dark:border-slate-800 bg-white/80 dark:bg-slate-900/80 backdrop-blur-md">
          <div className="container mx-auto px-4 h-16 flex items-center justify-between">
            <Link href="/" className="flex items-center space-x-2">
              <Stethoscope className="w-6 h-6 text-[var(--color-primary-600)]" />
              <span className="font-bold text-xl tracking-tight text-[var(--color-primary-600)] dark:text-sky-400">
                MedicHp
              </span>
            </Link>
            <nav className="hidden md:flex gap-6 font-medium text-sm">
              <Link href="/search" className="hover:text-[var(--color-primary-600)] transition-colors">Find a Doctor</Link>
              <Link href="/specialties" className="hover:text-[var(--color-primary-600)] transition-colors">Specialties</Link>
              <Link href="/about" className="hover:text-[var(--color-primary-600)] transition-colors">About Us</Link>
            </nav>
            <div className="flex items-center gap-4">
              <Link href="/login" className="text-sm font-medium hover:text-[var(--color-primary-600)]">Log in</Link>
              <Link href="/register" className="text-sm font-medium bg-[var(--color-primary-600)] text-white px-4 py-2 rounded-lg hover:opacity-90 transition-opacity">
                Sign up
              </Link>
            </div>
          </div>
        </header>
        <main className="flex-1 flex flex-col">{children}</main>
        <footer className="border-t border-gray-200 dark:border-slate-800 py-8 bg-white dark:bg-slate-900 mt-auto">
          <div className="container mx-auto px-4 text-center text-sm text-gray-500 dark:text-gray-400">
            &copy; {new Date().getFullYear()} MedicHp. All rights reserved.
          </div>
        </footer>
      </body>
    </html>
  );
}

