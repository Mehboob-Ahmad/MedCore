import Link from "next/link";
import { Stethoscope, Home, ArrowLeft, Calendar, FileQuestion, Mail } from "lucide-react";
import { Button } from "@medichp/ui";
import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Page Not Found (404)",
  description: "The page you are looking for does not exist or has been moved.",
};

export default function NotFound() {
  return (
    <div className="flex-1 min-h-[70vh] flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] from-sky-100/40 via-transparent to-transparent dark:from-sky-950/20">
      <div className="w-full max-w-xl text-center py-12 px-6">
        {/* Animated Icon Badge */}
        <div className="inline-flex items-center justify-center w-20 h-20 rounded-2xl bg-sky-100 dark:bg-sky-900/40 text-[var(--color-primary-600)] dark:text-sky-400 mb-6 shadow-inner ring-1 ring-sky-200 dark:ring-sky-800">
          <FileQuestion className="w-10 h-10" />
        </div>

        <div className="space-y-2 mb-8">
          <span className="text-xs font-bold uppercase tracking-widest text-[var(--color-primary-600)] dark:text-sky-400 bg-sky-50 dark:bg-sky-950/50 px-3 py-1 rounded-full border border-sky-200 dark:border-sky-800">
            Error 404
          </span>
          <h1 className="text-3xl sm:text-4xl font-extrabold text-gray-900 dark:text-white tracking-tight mt-3">
            Page Not Found
          </h1>
          <p className="text-base text-gray-600 dark:text-gray-300 max-w-md mx-auto">
            We couldn&apos;t find the page you were looking for. It might have been moved, renamed, or temporarily unavailable.
          </p>
        </div>

        {/* Quick Action Navigation */}
        <div className="flex flex-col sm:flex-row items-center justify-center gap-3 mb-10">
          <Link href="/" className="w-full sm:w-auto">
            <Button size="lg" className="w-full sm:w-auto flex items-center justify-center gap-2 bg-[var(--color-primary-600)] hover:bg-blue-700">
              <Home className="w-4 h-4" />
              <span>Return Home</span>
            </Button>
          </Link>
          <Link href="/request-demo" className="w-full sm:w-auto">
            <Button size="lg" variant="outline" className="w-full sm:w-auto flex items-center justify-center gap-2">
              <Calendar className="w-4 h-4" />
              <span>Request a Demo</span>
            </Button>
          </Link>
        </div>

        {/* Helpful Shortcuts */}
        <div className="pt-6 border-t border-gray-200 dark:border-slate-800 text-sm text-gray-500 dark:text-gray-400">
          <p className="mb-3">Looking for something specific?</p>
          <div className="flex flex-wrap justify-center gap-4 text-[var(--color-primary-600)] dark:text-sky-400 font-medium">
            <Link href="/features" className="hover:underline">Features Overview</Link>
            <span>&bull;</span>
            <Link href="/login" className="hover:underline">Sign In</Link>
            <span>&bull;</span>
            <Link href="/register/doctor" className="hover:underline">Doctor Registration</Link>
            <span>&bull;</span>
            <a href="mailto:medcore.pk.official@gmail.com" className="inline-flex items-center gap-1 hover:underline">
              <Mail className="w-3.5 h-3.5" /> Contact Support
            </a>
          </div>
        </div>
      </div>
    </div>
  );
}
