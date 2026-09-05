"use client";

import Link from "next/link";
import { motion } from "framer-motion";
import { Stethoscope, UserRound, BriefcaseMedical, ArrowLeft } from "lucide-react";
import { Card, CardContent } from "@medichp/ui";

export default function RegisterPage() {
  return (
    <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-sky-100/40 via-transparent to-transparent dark:from-sky-900/10">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="w-full max-w-lg my-12"
      >
        <div className="text-center mb-8">
          <Link href="/" className="inline-flex items-center space-x-2">
            <Stethoscope className="w-8 h-8 text-[var(--color-primary-600)]" />
            <span className="font-bold text-2xl tracking-tight text-[var(--color-primary-600)] dark:text-sky-400">
              MedicHp
            </span>
          </Link>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Create your MedicHp Account</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2">Choose how you want to use MedicHp.</p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
          {/* Patient Card */}
          <Link href="/register/patient">
            <motion.div whileHover={{ y: -4 }} whileTap={{ scale: 0.98 }}>
              <Card className="border-2 border-transparent hover:border-[var(--color-primary-600)] transition-all cursor-pointer h-full shadow-lg hover:shadow-xl">
                <CardContent className="pt-8 pb-8 flex flex-col items-center text-center gap-4">
                  <div className="w-16 h-16 rounded-full bg-sky-100 dark:bg-sky-900/30 flex items-center justify-center">
                    <UserRound className="w-8 h-8 text-[var(--color-primary-600)]" />
                  </div>
                  <div>
                    <h2 className="text-lg font-bold text-gray-900 dark:text-white">Sign up as Patient</h2>
                    <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                      Find doctors, book appointments, and manage your health.
                    </p>
                  </div>
                  <span className="text-sm font-semibold text-[var(--color-primary-600)] mt-2">
                    Continue →
                  </span>
                </CardContent>
              </Card>
            </motion.div>
          </Link>

          {/* Doctor Card */}
          <Link href="/register/doctor">
            <motion.div whileHover={{ y: -4 }} whileTap={{ scale: 0.98 }}>
              <Card className="border-2 border-transparent hover:border-slate-800 dark:hover:border-slate-400 transition-all cursor-pointer h-full shadow-lg hover:shadow-xl">
                <CardContent className="pt-8 pb-8 flex flex-col items-center text-center gap-4">
                  <div className="w-16 h-16 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center">
                    <BriefcaseMedical className="w-8 h-8 text-slate-700 dark:text-slate-300" />
                  </div>
                  <div>
                    <h2 className="text-lg font-bold text-gray-900 dark:text-white">Sign up as Doctor</h2>
                    <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                      Join MedicHp to grow your practice and connect with patients.
                    </p>
                  </div>
                  <span className="text-sm font-semibold text-slate-700 dark:text-slate-300 mt-2">
                    Continue →
                  </span>
                </CardContent>
              </Card>
            </motion.div>
          </Link>
        </div>

        <div className="text-center mt-8">
          <Link href="/login" className="inline-flex items-center gap-2 text-sm font-medium text-gray-600 dark:text-gray-400 hover:text-[var(--color-primary-600)] transition-colors">
            <ArrowLeft className="w-4 h-4" />
            Back to Login
          </Link>
        </div>
      </motion.div>
    </div>
  );
}
