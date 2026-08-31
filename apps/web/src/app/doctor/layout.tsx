import Link from "next/link";
import { LayoutDashboard, Calendar, MessageSquare, User, Clock } from "lucide-react";

export default function DoctorLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex h-full bg-slate-50 dark:bg-slate-900">
      {/* Sidebar Navigation */}
      <aside className="w-64 border-r border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 hidden md:block">
        <div className="p-6">
          <h2 className="text-xs uppercase font-bold text-gray-500 tracking-wider">Doctor Portal</h2>
        </div>
        <nav className="px-4 space-y-2">
          <Link href="/doctor/dashboard" className="flex items-center space-x-3 px-4 py-3 text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg">
            <LayoutDashboard className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
            <span className="font-medium">Dashboard</span>
          </Link>
          <Link href="/doctor/patients" className="flex items-center space-x-3 px-4 py-3 text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg">
            <User className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
            <span className="font-medium">Patients</span>
          </Link>
          <Link href="/doctor/schedule" className="flex items-center space-x-3 px-4 py-3 text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg">
            <Calendar className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
            <span className="font-medium">Schedule</span>
          </Link>
          <Link href="/doctor/messages" className="flex items-center space-x-3 px-4 py-3 text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg">
            <MessageSquare className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
            <span className="font-medium">Messages</span>
          </Link>
          <Link href="/doctor/profile" className="flex items-center space-x-3 px-4 py-3 text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg">
            <User className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
            <span className="font-medium">My Profile</span>
          </Link>
        </nav>
      </aside>

      {/* Main Content Area */}
      <main className="flex-1 flex flex-col min-h-0 overflow-y-auto">
        <div className="flex-1 p-6 lg:p-8">
          {children}
        </div>
      </main>
      
      {/* Mobile Bottom Tab Navigation - exactly mirroring the Expo router layout */}
      <div className="md:hidden fixed bottom-0 left-0 right-0 border-t border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 pb-safe z-50">
        <nav className="flex justify-around p-3">
          <Link href="/doctor/dashboard" className="flex flex-col items-center text-gray-500 hover:text-indigo-600">
            <LayoutDashboard className="w-6 h-6" />
            <span className="text-[10px] mt-1 font-medium">Dashboard</span>
          </Link>
          <Link href="/doctor/schedule" className="flex flex-col items-center text-gray-500 hover:text-indigo-600">
            <Calendar className="w-6 h-6" />
            <span className="text-[10px] mt-1 font-medium">Schedule</span>
          </Link>
          <Link href="/doctor/messages" className="flex flex-col items-center text-gray-500 hover:text-indigo-600">
            <MessageSquare className="w-6 h-6" />
            <span className="text-[10px] mt-1 font-medium">Inbox</span>
          </Link>
          <Link href="/doctor/profile" className="flex flex-col items-center text-gray-500 hover:text-indigo-600">
            <User className="w-6 h-6" />
            <span className="text-[10px] mt-1 font-medium">Profile</span>
          </Link>
        </nav>
      </div>
    </div>
  );
}
