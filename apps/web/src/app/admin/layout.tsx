"use client";

import Link from "next/link";
import { LayoutDashboard, Users, UserPlus, Settings, ShieldAlert, LogOut } from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const { logout } = useAuth();

  return (
    <div className="flex h-full bg-slate-50 dark:bg-slate-900">
      {/* Sidebar Navigation */}
      <aside className="w-64 border-r border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 flex flex-col hidden md:flex">
        <div className="p-6">
          <h2 className="text-xs uppercase font-bold text-gray-500 tracking-wider">Super Admin</h2>
        </div>
        <nav className="px-4 space-y-2 flex-1">
          <Link href="/admin/dashboard" className="flex items-center space-x-3 px-4 py-3 bg-red-50 dark:bg-red-900/10 text-red-700 dark:text-red-400 rounded-lg">
            <LayoutDashboard className="w-5 h-5" />
            <span className="font-medium">Overview</span>
          </Link>
          <Link href="/admin/invite" className="flex items-center space-x-3 px-4 py-3 text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg">
            <UserPlus className="w-5 h-5" />
            <span className="font-medium">Invite Admin</span>
          </Link>
          <Link href="/admin/settings" className="flex items-center space-x-3 px-4 py-3 text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg">
            <Settings className="w-5 h-5" />
            <span className="font-medium">Settings</span>
          </Link>
        </nav>
        
        {/* Bottom Sidebar - Sign Out */}
        <div className="p-4 border-t border-gray-200 dark:border-slate-800 mt-auto">
          <button 
            onClick={() => logout()}
            className="flex items-center space-x-3 px-4 py-3 w-full text-gray-700 dark:text-gray-300 hover:bg-red-50 hover:text-red-600 dark:hover:bg-slate-800 rounded-lg transition-colors"
          >
            <LogOut className="w-5 h-5" />
            <span className="font-medium">Sign Out</span>
          </button>
        </div>
      </aside>

      {/* Main Content Area */}
      <main className="flex-1 flex flex-col min-h-0 overflow-y-auto">
        <div className="flex-1 p-6 lg:p-8">
          {children}
        </div>
      </main>
      
      {/* Mobile Bottom Tab Navigation */}
      <div className="md:hidden fixed bottom-0 left-0 right-0 border-t border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 pb-safe z-50">
        <nav className="flex justify-around p-3">
          <Link href="/admin/dashboard" className="flex flex-col items-center text-red-600">
            <LayoutDashboard className="w-6 h-6" />
            <span className="text-[10px] mt-1 font-medium">Overview</span>
          </Link>
          <Link href="/admin/settings" className="flex flex-col items-center text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
            <Settings className="w-6 h-6" />
            <span className="text-[10px] mt-1 font-medium">Settings</span>
          </Link>
        </nav>
      </div>
    </div>
  );
}
