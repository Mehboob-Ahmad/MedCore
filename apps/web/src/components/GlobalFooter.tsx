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

import { Logo } from "@/components/Logo";

export function GlobalFooter() {
  const pathname = usePathname();
  const isDashboard = pathname?.startsWith("/patient") || pathname?.startsWith("/doctor") || pathname?.startsWith("/admin");

  if (isDashboard) return null;

  return (
    <footer id="contacts" className="border-t border-gray-200 dark:border-slate-800 py-12 bg-white dark:bg-slate-900 mt-auto">
      <div className="container mx-auto px-4">
        <div className="flex flex-col md:flex-row items-center justify-between gap-8 pb-8 border-b border-gray-100 dark:border-slate-800 text-center md:text-left">
          <div className="space-y-3">
            <Logo size={32} />
            <p className="text-sm text-gray-500 dark:text-gray-400 max-w-sm">
              Empowering healthcare professionals and patients with next-generation digital tools.
            </p>
          </div>

          <div className="flex flex-wrap justify-center gap-6 text-sm font-medium text-gray-600 dark:text-gray-300">
            <a href="/features" className="hover:text-[var(--color-primary-600)] transition-colors">Features</a>
            <a href="/request-demo" className="hover:text-[var(--color-primary-600)] transition-colors">Request a Demo</a>
            <a href="/register/doctor" className="hover:text-[var(--color-primary-600)] transition-colors">Doctor Onboarding</a>
            <a href="/login" className="hover:text-[var(--color-primary-600)] transition-colors">Sign In</a>
          </div>

          <div className="flex flex-col sm:flex-row items-center gap-4">
            {/* Clickable mail with clear label and icon */}
            <a 
              href="mailto:medcore.pk.official@gmail.com" 
              className="inline-flex items-center gap-2 px-3.5 py-2 rounded-lg bg-blue-50 dark:bg-slate-800 text-sm font-medium text-[var(--color-primary-600)] dark:text-sky-400 hover:bg-blue-100 dark:hover:bg-slate-700 transition-colors"
              title="Send us an email"
            >
              <Mail className="w-4 h-4 shrink-0" />
              <span>medcore.pk.official@gmail.com</span>
            </a>

            <a 
              href="https://www.instagram.com/medichp_com/?hl=en" 
              target="_blank" 
              rel="noopener noreferrer" 
              className="inline-flex items-center gap-2 px-3 py-2 rounded-lg text-gray-500 hover:text-pink-600 hover:bg-pink-50 dark:hover:bg-pink-950/30 transition-colors text-sm font-medium"
              title="Follow us on Instagram"
            >
              <InstagramIcon className="w-4 h-4 shrink-0" />
              <span className="hidden sm:inline">Instagram</span>
            </a>
          </div>
        </div>

        <div className="pt-6 flex flex-col sm:flex-row items-center justify-between gap-4 text-xs text-gray-500 dark:text-gray-400">
          <div>
            &copy; {new Date().getFullYear()} MedicHp Systems. All rights reserved.
          </div>
          <div className="flex items-center gap-4">
            <span>HIPAA-Aligned Architecture</span>
            <span>&bull;</span>
            <span>Enterprise Security</span>
          </div>
        </div>
      </div>
    </footer>
  );
}
