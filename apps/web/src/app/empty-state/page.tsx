"use client";

import { useState } from "react";
import { EmptyState } from "@/components/EmptyState";
import { CalendarX, MessageSquareOff, FileText, Search, UserX } from "lucide-react";
import Link from "next/link";
import { Button } from "@medichp/ui";

export default function EmptyStateDemoPage() {
  const [activeTab, setActiveTab] = useState<"appointments" | "records" | "messages" | "search">("appointments");

  return (
    <div className="container mx-auto px-4 py-12 max-w-4xl">
      <div className="text-center mb-8">
        <h1 className="text-3xl font-extrabold text-gray-900 dark:text-white">Empty State Showcase</h1>
        <p className="text-gray-500 dark:text-gray-400 mt-2">
          Previewing responsive empty states designed for zero-data scenarios across portals.
        </p>
      </div>

      {/* Tabs */}
      <div className="flex flex-wrap justify-center gap-2 mb-10 border-b border-gray-200 dark:border-slate-800 pb-4">
        <button
          onClick={() => setActiveTab("appointments")}
          className={`px-4 py-2 rounded-lg font-medium text-sm transition-all ${
            activeTab === "appointments"
              ? "bg-[var(--color-primary-600)] text-white shadow-sm"
              : "text-gray-600 dark:text-gray-400 hover:bg-slate-100 dark:hover:bg-slate-800"
          }`}
        >
          Appointments
        </button>
        <button
          onClick={() => setActiveTab("records")}
          className={`px-4 py-2 rounded-lg font-medium text-sm transition-all ${
            activeTab === "records"
              ? "bg-[var(--color-primary-600)] text-white shadow-sm"
              : "text-gray-600 dark:text-gray-400 hover:bg-slate-100 dark:hover:bg-slate-800"
          }`}
        >
          Medical Records
        </button>
        <button
          onClick={() => setActiveTab("messages")}
          className={`px-4 py-2 rounded-lg font-medium text-sm transition-all ${
            activeTab === "messages"
              ? "bg-[var(--color-primary-600)] text-white shadow-sm"
              : "text-gray-600 dark:text-gray-400 hover:bg-slate-100 dark:hover:bg-slate-800"
          }`}
        >
          Inbox / Messages
        </button>
        <button
          onClick={() => setActiveTab("search")}
          className={`px-4 py-2 rounded-lg font-medium text-sm transition-all ${
            activeTab === "search"
              ? "bg-[var(--color-primary-600)] text-white shadow-sm"
              : "text-gray-600 dark:text-gray-400 hover:bg-slate-100 dark:hover:bg-slate-800"
          }`}
        >
          Search Results
        </button>
      </div>

      {/* Tab Panels */}
      <div className="py-6">
        {activeTab === "appointments" && (
          <EmptyState
            icon={<CalendarX className="w-8 h-8 stroke-[1.75]" />}
            title="No Scheduled Appointments"
            description="You don't have any appointments scheduled for this date. Book your next consultation or check upcoming slots."
            actionText="Book New Appointment"
            actionHref="/patient/search"
            secondaryActionText="View Past History"
            secondaryActionHref="/patient/dashboard"
          />
        )}

        {activeTab === "records" && (
          <EmptyState
            icon={<FileText className="w-8 h-8 stroke-[1.75]" />}
            title="No Medical Records Yet"
            description="Diagnostic test results, digital prescriptions, and doctor consultation notes will automatically appear here once issued."
            actionText="Request a Demo"
            actionHref="/request-demo"
            secondaryActionText="Return to Home"
            secondaryActionHref="/"
          />
        )}

        {activeTab === "messages" && (
          <EmptyState
            icon={<MessageSquareOff className="w-8 h-8 stroke-[1.75]" />}
            title="No Messages Found"
            description="Your message thread is currently empty. Start a conversation with your healthcare provider after an appointment."
            actionText="Find a Doctor"
            actionHref="/patient/search"
          />
        )}

        {activeTab === "search" && (
          <EmptyState
            icon={<Search className="w-8 h-8 stroke-[1.75]" />}
            title="No Doctors Match Your Search"
            description="We couldn't find any healthcare professionals matching those filters. Try searching by specialty, city, or general terms."
            actionText="Clear Filters"
            actionHref="/patient/search"
            secondaryActionText="Explore Specialties"
            secondaryActionHref="/features"
          />
        )}
      </div>

      <div className="mt-12 text-center">
        <Link href="/">
          <Button variant="outline" size="sm">
            &larr; Back to Main Website
          </Button>
        </Link>
      </div>
    </div>
  );
}
