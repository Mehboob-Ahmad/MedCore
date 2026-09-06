"use client";

import { usePathname } from "next/navigation";
import { Instagram, Mail } from "lucide-react";

export function GlobalFooter() {
  const pathname = usePathname();
  const isDashboard = pathname?.startsWith("/patient") || pathname?.startsWith("/doctor") || pathname?.startsWith("/admin");

  if (isDashboard) return null;

  return (
    <footer className="border-t border-gray-200 dark:border-slate-800 py-8 bg-white dark:bg-slate-900 mt-auto">
      <div className="container mx-auto px-4 flex flex-col items-center justify-center space-y-4">
        <div className="flex space-x-6">
          <a href="https://www.instagram.com/medichp_com/?hl=en" target="_blank" rel="noopener noreferrer" className="text-gray-500 hover:text-pink-600 transition-colors flex items-center gap-2">
            <Instagram className="w-5 h-5" />
            <span className="sr-only">Instagram</span>
          </a>
          <a href="mailto:medcore.pk.official@gmail.com" className="text-gray-500 hover:text-[var(--color-primary-600)] transition-colors flex items-center gap-2">
            <Mail className="w-5 h-5" />
            <span className="sr-only">Email</span>
          </a>
        </div>
        <div className="text-sm text-gray-500 dark:text-gray-400">
          &copy; {new Date().getFullYear()} MedicHp. All rights reserved.
        </div>
      </div>
    </footer>
  );
}
