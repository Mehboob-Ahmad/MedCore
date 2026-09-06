"use client";

import { usePathname } from "next/navigation";
import { Mail } from "lucide-react";

const InstagramIcon = ({ className }: { className?: string }) => (
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width="24"
    height="24"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth="2"
    strokeLinecap="round"
    strokeLinejoin="round"
    className={className}
  >
    <rect width="20" height="20" x="2" y="2" rx="5" ry="5" />
    <path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z" />
    <line x1="17.5" x2="17.51" y1="6.5" y2="6.5" />
  </svg>
);

export function GlobalFooter() {
  const pathname = usePathname();
  const isDashboard = pathname?.startsWith("/patient") || pathname?.startsWith("/doctor") || pathname?.startsWith("/admin");

  if (isDashboard) return null;

  return (
    <footer className="border-t border-gray-200 dark:border-slate-800 py-8 bg-white dark:bg-slate-900 mt-auto">
      <div className="container mx-auto px-4 flex flex-col items-center justify-center space-y-4">
        <div className="flex space-x-6">
          <a href="https://www.instagram.com/medichp_com/?hl=en" target="_blank" rel="noopener noreferrer" className="text-gray-500 hover:text-pink-600 transition-colors flex items-center gap-2">
            <InstagramIcon className="w-5 h-5" />
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
