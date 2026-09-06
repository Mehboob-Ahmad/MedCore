"use client";

import { motion } from "framer-motion";
import { Activity, Users, MessageSquare, Brain, Calendar, ShieldCheck } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@medichp/ui";
import Link from "next/link";

export default function FeaturesPage() {
  return (
    <div className="flex flex-col items-center">
      {/* Hero Section */}
      <section className="w-full py-20 bg-gradient-to-b from-blue-50 to-white dark:from-slate-900 dark:to-slate-800 text-center">
        <div className="container mx-auto px-4 max-w-4xl">
          <motion.h1 
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="text-4xl md:text-5xl font-bold tracking-tight text-gray-900 dark:text-white mb-6"
          >
            Powerful Features for Modern Healthcare
          </motion.h1>
          <motion.p 
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 }}
            className="text-lg text-gray-600 dark:text-gray-300 max-w-2xl mx-auto"
          >
            MedicHp bridges the gap between doctors and patients using intelligent systems, seamless communication, and secure medical records.
          </motion.p>
        </div>
      </section>

      {/* Doctor Features Section */}
      <section className="w-full py-16 bg-white dark:bg-slate-900">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 dark:text-white mb-4">Doctor Features</h2>
            <p className="text-gray-500 dark:text-gray-400 max-w-2xl mx-auto text-lg">
              Empowering healthcare providers with tools to efficiently manage and check their patients.
            </p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8 max-w-6xl mx-auto">
            {/* Feature 1 */}
            <Card className="border border-gray-100 dark:border-slate-800 shadow-sm hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="h-10 w-10 rounded-lg bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 flex items-center justify-center mb-4">
                  <Users className="h-5 w-5" />
                </div>
                <CardTitle>Comprehensive Patient Profiles</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base text-gray-600 dark:text-gray-300">
                  Access complete medical histories, previous prescriptions, and lab reports instantly before you even see the patient, saving valuable consultation time.
                </CardDescription>
              </CardContent>
            </Card>

            {/* Feature 2 */}
            <Card className="border border-gray-100 dark:border-slate-800 shadow-sm hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="h-10 w-10 rounded-lg bg-emerald-100 dark:bg-emerald-900/30 text-emerald-600 dark:text-emerald-400 flex items-center justify-center mb-4">
                  <Brain className="h-5 w-5" />
                </div>
                <CardTitle>Specialized AI Assistant</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base text-gray-600 dark:text-gray-300">
                  Leverage our built-in Doctor AI strictly tuned to your medical specialization. It helps quickly summarize patient histories and highlights potential red flags in medical records.
                </CardDescription>
              </CardContent>
            </Card>

            {/* Feature 3 */}
            <Card className="border border-gray-100 dark:border-slate-800 shadow-sm hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="h-10 w-10 rounded-lg bg-purple-100 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400 flex items-center justify-center mb-4">
                  <MessageSquare className="h-5 w-5" />
                </div>
                <CardTitle>Persistent Patient Chat</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base text-gray-600 dark:text-gray-300">
                  Communicate securely with your patients through built-in messaging. Send text, voice notes, and images directly without exposing your personal phone number.
                </CardDescription>
              </CardContent>
            </Card>

            {/* Feature 4 */}
            <Card className="border border-gray-100 dark:border-slate-800 shadow-sm hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="h-10 w-10 rounded-lg bg-orange-100 dark:bg-orange-900/30 text-orange-600 dark:text-orange-400 flex items-center justify-center mb-4">
                  <Calendar className="h-5 w-5" />
                </div>
                <CardTitle>Smart Appointment Management</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base text-gray-600 dark:text-gray-300">
                  Automate your scheduling. Patients can view your real-time availability and book slots directly, reducing administrative overhead for your clinic.
                </CardDescription>
              </CardContent>
            </Card>

            {/* Feature 5 */}
            <Card className="border border-gray-100 dark:border-slate-800 shadow-sm hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="h-10 w-10 rounded-lg bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400 flex items-center justify-center mb-4">
                  <ShieldCheck className="h-5 w-5" />
                </div>
                <CardTitle>Secure Data Isolation</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base text-gray-600 dark:text-gray-300">
                  Strict role-based access ensures that you only see your authorized patients, and your practice's data is completely isolated and secure.
                </CardDescription>
              </CardContent>
            </Card>
            
            {/* Feature 6 */}
            <Card className="border border-gray-100 dark:border-slate-800 shadow-sm hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="h-10 w-10 rounded-lg bg-sky-100 dark:bg-sky-900/30 text-sky-600 dark:text-sky-400 flex items-center justify-center mb-4">
                  <Activity className="h-5 w-5" />
                </div>
                <CardTitle>Automated Notifications</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base text-gray-600 dark:text-gray-300">
                  The system automatically sends WhatsApp and Email reminders to your patients for upcoming appointments and pending fees, ensuring lower no-show rates.
                </CardDescription>
              </CardContent>
            </Card>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="w-full py-20 bg-[var(--color-primary-600)] text-white text-center">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl md:text-4xl font-bold mb-6">See MedicHp in Action</h2>
          <p className="text-blue-100 mb-10 max-w-2xl mx-auto text-lg">
            Ready to upgrade your medical practice? Request a demo today and experience how our features can help you serve your patients better.
          </p>
          <Link href="/request-demo">
            <Button size="lg" className="bg-white text-[var(--color-primary-600)] hover:bg-gray-100 px-10 h-14 text-lg">
              Request a Demo
            </Button>
          </Link>
        </div>
      </section>
    </div>
  );
}
