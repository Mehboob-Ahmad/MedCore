"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, Calendar, MessageSquare, User, Clock } from "lucide-react";
import { ProtectedRoute } from "@/components/ProtectedRoute";

export default function DoctorLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();

  const isActive = (path: string) => {
    if (path === "/doctor/dashboard" && pathname === "/doctor/dashboard") return true;
    if (path !== "/doctor/dashboard" && pathname.startsWith(path)) return true;
    return false;
  };
  
  const baseDesktopLinkClass = "flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors duration-200";
  const inactiveDesktopClass = "text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800";
  const activeDesktopClass = "bg-indigo-50 dark:bg-slate-800 text-indigo-600 dark:text-indigo-400 font-semibold ring-1 ring-indigo-600 dark:ring-indigo-400";

  const getDesktopLinkClass = (path: string) => {
    return `${baseDesktopLinkClass} ${isActive(path) ? activeDesktopClass : inactiveDesktopClass}`;
  };

  const baseMobileLinkClass = "flex flex-col items-center transition-colors duration-200 px-3 py-2 rounded-lg";
  const inactiveMobileClass = "text-gray-500 hover:text-indigo-600 dark:hover:text-indigo-400";
  const activeMobileClass = "text-indigo-600 dark:text-indigo-400 bg-indigo-50 dark:bg-slate-800";

  const getMobileLinkClass = (path: string) => {
    return `${baseMobileLinkClass} ${isActive(path) ? activeMobileClass : inactiveMobileClass}`;
  };

  return (
    <ProtectedRoute allowedRoles={["Doctor"]}>
      <div className="flex h-full bg-slate-50 dark:bg-slate-900">
        {/* Sidebar Navigation */}
        <aside className="w-64 border-r border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 hidden md:block">
          <div className="p-6">
            <h2 className="text-xs uppercase font-bold text-gray-500 tracking-wider">Doctor Portal</h2>
          </div>
          <nav className="px-4 space-y-2">
            <Link href="/doctor/dashboard" className={getDesktopLinkClass("/doctor/dashboard")}>
              <LayoutDashboard className="w-5 h-5" />
              <span>Dashboard</span>
            </Link>
            <Link href="/doctor/patients" className={getDesktopLinkClass("/doctor/patients")}>
              <User className="w-5 h-5" />
              <span>Patients</span>
            </Link>
            <Link href="/doctor/schedule" className={getDesktopLinkClass("/doctor/schedule")}>
              <Calendar className="w-5 h-5" />
              <span>Schedule</span>
            </Link>
            <Link href="/doctor/messages" className={getDesktopLinkClass("/doctor/messages")}>
              <MessageSquare className="w-5 h-5" />
              <span>Messages</span>
            </Link>
            <Link href="/doctor/profile" className={getDesktopLinkClass("/doctor/profile")}>
              <User className="w-5 h-5" />
              <span>My Profile</span>
            </Link>
          </nav>
        </aside>

        {/* Main Content Area */}
        <main className="flex-1 flex flex-col min-h-0 overflow-y-auto pb-20 md:pb-0">
          <div className="flex-1 p-6 lg:p-8">
            {children}
          </div>
        </main>
        
        {/* Mobile Bottom Tab Navigation */}
        <div className="md:hidden fixed bottom-0 left-0 right-0 border-t border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 pb-safe z-50">
          <nav className="flex justify-around p-2">
            <Link href="/doctor/dashboard" className={getMobileLinkClass("/doctor/dashboard")}>
              <LayoutDashboard className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Dashboard</span>
            </Link>
            <Link href="/doctor/schedule" className={getMobileLinkClass("/doctor/schedule")}>
              <Calendar className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Schedule</span>
            </Link>
            <Link href="/doctor/messages" className={getMobileLinkClass("/doctor/messages")}>
              <MessageSquare className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Inbox</span>
            </Link>
            <Link href="/doctor/profile" className={getMobileLinkClass("/doctor/profile")}>
              <User className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Profile</span>
            </Link>
          </nav>
        </div>
      </div>
    </ProtectedRoute>
  );
}
