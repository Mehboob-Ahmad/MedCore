"use client";

import { Bell } from "lucide-react";

export default function PatientNotificationsPage() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6 flex items-center gap-2">
        <Bell className="w-6 h-6 text-[var(--color-primary-600)]" /> Notifications & Alerts
      </h1>

      <div className="bg-white dark:bg-slate-800 rounded-lg shadow p-8 text-center">
        <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-slate-100 dark:bg-slate-700 mb-4">
          <Bell className="w-8 h-8 text-slate-400" />
        </div>
        <h3 className="text-lg font-medium text-slate-900 dark:text-slate-100 mb-1">No new notifications</h3>
        <p className="text-slate-500 dark:text-slate-400">
          You're all caught up! We'll notify you when there's an update regarding your appointments or messages.
        </p>
      </div>
    </div>
  );
}
