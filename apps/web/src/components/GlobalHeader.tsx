"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Stethoscope, Menu, X, LogOut, User } from "lucide-react";
import { NavbarAuthMenu } from "@/components/NavbarAuthMenu";
import { useAuth } from "@/contexts/AuthContext";
import { motion, AnimatePresence } from "framer-motion";

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
        <Link href={dashboardLink} className="flex items-center space-x-2" onClick={() => setMobileMenuOpen(false)}>
          <Stethoscope className="w-6 h-6 text-[var(--color-primary-600)]" />
          <span className="font-bold text-xl tracking-tight text-[var(--color-primary-600)] dark:text-sky-400">
            MedicHp
          </span>
        </Link>
        
        {/* Desktop Navigation */}
        <nav className="hidden lg:flex items-center gap-6 font-medium text-sm">
          <Link href="/" className="hover:text-[var(--color-primary-600)] transition-colors">Home</Link>
          <Link href="/about" className="hover:text-[var(--color-primary-600)] transition-colors">About</Link>
          <Link href="/features" className="hover:text-[var(--color-primary-600)] transition-colors">Features</Link>
          <Link href="/for-doctors" className="hover:text-[var(--color-primary-600)] transition-colors">For Doctors</Link>
          <Link href="/for-patients" className="hover:text-[var(--color-primary-600)] transition-colors">For Patients</Link>
          <Link 
            href="/request-demo" 
            className="text-[var(--color-primary-600)] dark:text-sky-400 font-bold hover:opacity-80 transition-opacity"
          >
            Request a Demo
          </Link>
          <NavbarAuthMenu />
        </nav>

        {/* Mobile Menu Toggle */}
        <button 
          className="md:hidden p-2 -mr-2 text-gray-600 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-colors"
          onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
          aria-label="Toggle Menu"
        >
          {mobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
        </button>
      </div>

      {/* Mobile Navigation Dropdown */}
      <AnimatePresence>
        {mobileMenuOpen && (
          <motion.div 
            initial={{ opacity: 0, height: 0 }}
            animate={{ opacity: 1, height: "auto" }}
            exit={{ opacity: 0, height: 0 }}
            className="md:hidden border-t border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 overflow-hidden shadow-lg"
          >
            <nav className="container mx-auto px-4 py-4 flex flex-col space-y-4">
              
              <Link href="/" className="font-medium text-gray-700 dark:text-gray-300 py-2 border-b border-gray-100 dark:border-slate-800" onClick={() => setMobileMenuOpen(false)}>Home</Link>
              <Link href="/about" className="font-medium text-gray-700 dark:text-gray-300 py-2 border-b border-gray-100 dark:border-slate-800" onClick={() => setMobileMenuOpen(false)}>About</Link>
              <Link href="/features" className="font-medium text-gray-700 dark:text-gray-300 py-2 border-b border-gray-100 dark:border-slate-800" onClick={() => setMobileMenuOpen(false)}>Features</Link>
              <Link href="/for-doctors" className="font-medium text-gray-700 dark:text-gray-300 py-2 border-b border-gray-100 dark:border-slate-800" onClick={() => setMobileMenuOpen(false)}>For Doctors</Link>
              <Link href="/for-patients" className="font-medium text-gray-700 dark:text-gray-300 py-2 border-b border-gray-100 dark:border-slate-800" onClick={() => setMobileMenuOpen(false)}>For Patients</Link>
              <Link 
                href="/request-demo" 
                className="font-bold text-[var(--color-primary-600)] py-2 border-b border-gray-100 dark:border-slate-800"
                onClick={() => setMobileMenuOpen(false)}
              >
                Request a Demo
              </Link>
              
              {isAuthenticated && user ? (
                <>
                  <Link 
                    href={dashboardLink} 
                    className="flex items-center gap-3 font-medium text-gray-700 dark:text-gray-300 py-2"
                    onClick={() => setMobileMenuOpen(false)}
                  >
                    <User className="w-5 h-5 text-[var(--color-primary-600)]" />
                    <span>{user.firstName} {user.lastName}</span>
                  </Link>
                  <button
                    onClick={handleLogout}
                    className="flex items-center gap-3 font-medium text-red-600 hover:text-red-700 py-2 text-left"
                  >
                    <LogOut className="w-5 h-5" />
                    <span>Log out</span>
                  </button>
                </>
              ) : (
                <div className="flex flex-col space-y-3 pt-2">
                  <Link 
                    href="/login" 
                    className="font-medium text-center py-2 text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-slate-700 rounded-lg"
                    onClick={() => setMobileMenuOpen(false)}
                  >
                    Log in
                  </Link>
                  <Link 
                    href="/register" 
                    className="font-medium text-center py-2 bg-[var(--color-primary-600)] text-white rounded-lg"
                    onClick={() => setMobileMenuOpen(false)}
                  >
                    Sign up
                  </Link>
                </div>
              )}
            </nav>
          </motion.div>
        )}
      </AnimatePresence>
    </header>
  );
}
