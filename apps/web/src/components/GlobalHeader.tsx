"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Stethoscope, Menu, X, LogOut, User } from "lucide-react";
import { NavbarAuthMenu } from "@/components/NavbarAuthMenu";
import { useAuth } from "@/contexts/AuthContext";
import { motion, AnimatePresence } from "framer-motion";

import { Logo } from "@/components/Logo";

export function GlobalHeader() {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const { user, isAuthenticated, role, logout } = useAuth();
  const pathname = usePathname();

  // Close menu when route changes
  useEffect(() => {
    setMobileMenuOpen(false);
  }, [pathname]);

  const handleLogout = async () => {
    try {
      await logout();
      setMobileMenuOpen(false);
    } catch (e) {
      console.error(e);
    }
  };

  let dashboardLink = "/";
  if (role === "Doctor") dashboardLink = "/doctor/dashboard";
  else if (role === "Patient") dashboardLink = "/patient/dashboard";
  else if (role === "SystemAdmin" || role === "Admin") dashboardLink = "/admin/dashboard";

  return (
    <header className="sticky top-0 z-50 w-full border-b border-gray-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md">
      <div className="container mx-auto px-4 h-16 flex items-center justify-between">
        <div className="flex-1 flex justify-start">
          <Logo href={dashboardLink} onClick={() => setMobileMenuOpen(false)} size={34} />
        </div>
        
        {/* Desktop Navigation */}
        <nav className="hidden lg:flex flex-1 items-center justify-center gap-8 font-medium text-sm">
          <Link href="/features" className="text-gray-700 dark:text-gray-300 hover:text-[var(--color-primary-600)] dark:hover:text-sky-400 transition-colors">Features</Link>
          <Link href="/request-demo" className="text-gray-700 dark:text-gray-300 hover:text-[var(--color-primary-600)] dark:hover:text-sky-400 transition-colors">Request a Demo</Link>
          <Link href="/#contacts" className="text-gray-700 dark:text-gray-300 hover:text-[var(--color-primary-600)] dark:hover:text-sky-400 transition-colors">Contacts</Link>
        </nav>

        <div className="flex-1 flex justify-end items-center gap-3">
          <div className="hidden lg:block">
            <NavbarAuthMenu />
          </div>
          {/* Mobile Menu Toggle - visible on < lg screens */}
          <button 
            type="button"
            className="lg:hidden p-2.5 -mr-2 text-gray-700 dark:text-gray-200 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-600)]"
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            aria-label="Toggle Menu"
            aria-expanded={mobileMenuOpen}
          >
            {mobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
          </button>
        </div>
      </div>

      {/* Mobile Navigation Dropdown */}
      <AnimatePresence>
        {mobileMenuOpen && (
          <motion.div 
            initial={{ opacity: 0, height: 0 }}
            animate={{ opacity: 1, height: "auto" }}
            exit={{ opacity: 0, height: 0 }}
            transition={{ duration: 0.25, ease: "easeInOut" }}
            className="lg:hidden border-t border-gray-200 dark:border-slate-800 bg-white/95 dark:bg-slate-900/95 backdrop-blur-md overflow-hidden shadow-xl"
          >
            <nav className="container mx-auto px-4 py-5 flex flex-col space-y-3">
              <Link 
                href="/features" 
                className="font-medium text-gray-800 dark:text-gray-200 py-2.5 px-3 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors flex items-center justify-between" 
                onClick={() => setMobileMenuOpen(false)}
              >
                <span>Features</span>
                <span className="text-xs text-gray-400">&rarr;</span>
              </Link>
              <Link 
                href="/request-demo" 
                className="font-medium text-gray-800 dark:text-gray-200 py-2.5 px-3 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors flex items-center justify-between" 
                onClick={() => setMobileMenuOpen(false)}
              >
                <span>Request a Demo</span>
                <span className="text-xs px-2 py-0.5 rounded-full bg-sky-100 dark:bg-sky-900/40 text-[var(--color-primary-600)] font-semibold">Doctor Onboarding</span>
              </Link>
              <Link 
                href="/#contacts" 
                className="font-medium text-gray-800 dark:text-gray-200 py-2.5 px-3 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors flex items-center justify-between" 
                onClick={() => setMobileMenuOpen(false)}
              >
                <span>Contacts &amp; Support</span>
                <span className="text-xs text-gray-400">&rarr;</span>
              </Link>

              <div className="pt-2 border-t border-gray-100 dark:border-slate-800">
                {isAuthenticated && user ? (
                  <div className="space-y-2">
                    <Link 
                      href={dashboardLink} 
                      className="flex items-center gap-3 font-semibold text-[var(--color-primary-600)] dark:text-sky-400 py-2.5 px-3 rounded-lg bg-sky-50 dark:bg-sky-950/30"
                      onClick={() => setMobileMenuOpen(false)}
                    >
                      <User className="w-5 h-5" />
                      <span>{user.firstName} {user.lastName} ({role || "Dashboard"})</span>
                    </Link>
                    <button
                      type="button"
                      onClick={handleLogout}
                      className="flex items-center gap-3 w-full font-medium text-red-600 hover:text-red-700 py-2.5 px-3 rounded-lg hover:bg-red-50 dark:hover:bg-red-950/20 text-left transition-colors"
                    >
                      <LogOut className="w-5 h-5" />
                      <span>Log out</span>
                    </button>
                  </div>
                ) : (
                  <div className="flex flex-col gap-2.5 pt-1">
                    <Link 
                      href="/login" 
                      className="w-full text-center py-2.5 rounded-lg border-2 border-[var(--color-primary-600)] text-[var(--color-primary-600)] dark:text-sky-400 dark:border-sky-400 font-semibold hover:bg-sky-50 dark:hover:bg-slate-800 transition-colors"
                      onClick={() => setMobileMenuOpen(false)}
                    >
                      Log in
                    </Link>
                    <Link 
                      href="/register/doctor" 
                      className="w-full text-center py-2.5 rounded-lg bg-[var(--color-primary-600)] text-white font-semibold shadow hover:bg-blue-700 transition-colors"
                      onClick={() => setMobileMenuOpen(false)}
                    >
                      Join as Doctor
                    </Link>
                  </div>
                )}
              </div>
            </nav>
          </motion.div>
        )}
      </AnimatePresence>
    </header>
  );
}
