"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Stethoscope } from "lucide-react";
import { NavbarAuthMenu } from "@/components/NavbarAuthMenu";

export function GlobalHeader() {
  const pathname = usePathname();
  const isDashboard = pathname?.startsWith("/patient") || pathname?.startsWith("/doctor") || pathname?.startsWith("/admin");

  if (isDashboard) return null;

  return (
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
        <NavbarAuthMenu />
      </div>
    </header>
  );
}
