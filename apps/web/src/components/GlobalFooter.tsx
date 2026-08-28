"use client";

import { usePathname } from "next/navigation";

export function GlobalFooter() {
  const pathname = usePathname();
  const isDashboard = pathname?.startsWith("/patient") || pathname?.startsWith("/doctor") || pathname?.startsWith("/admin");

  if (isDashboard) return null;

  return (
    <footer className="border-t border-gray-200 dark:border-slate-800 py-8 bg-white dark:bg-slate-900 mt-auto">
      <div className="container mx-auto px-4 text-center text-sm text-gray-500 dark:text-gray-400">
        &copy; {new Date().getFullYear()} MedicHp. All rights reserved.
      </div>
    </footer>
  );
}
