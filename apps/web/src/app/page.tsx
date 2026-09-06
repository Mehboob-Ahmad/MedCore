"use client";

import Image from "next/image";
import Link from "next/link";
import { motion, type Variants } from "framer-motion";
import { Search, CalendarCheck, FileText, Activity } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@medichp/ui";
import { Input } from "@medichp/ui";

const FADE_UP_ANIMATION_VARIANTS: Variants = {
  hidden: { opacity: 0, y: 30 },
  show: { opacity: 1, y: 0, transition: { type: "spring", stiffness: 100, damping: 20 } },
};

export default function Home() {
  return (
    <div className="flex flex-col items-center">
      {/* Hero Section */}
      <section className="relative w-full min-h-[600px] flex items-center justify-center overflow-hidden bg-gradient-to-b from-blue-50 to-white dark:from-slate-900 dark:to-slate-800">
        <div className="absolute inset-0 w-full h-full bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-sky-200/40 via-transparent to-transparent dark:from-sky-900/20" />
        
        <motion.div
          initial="hidden"
          animate="show"
          viewport={{ once: true }}
          variants={{
            hidden: {},
            show: {
              transition: { staggerChildren: 0.15 },
            },
          }}
          className="container mx-auto px-4 z-10 text-center max-w-4xl"
        >
          <motion.h1 
            variants={FADE_UP_ANIMATION_VARIANTS}
            className="text-5xl md:text-7xl font-bold tracking-tight text-gray-900 dark:text-white mb-6"
          >
            Digital healthcare, <br className="hidden md:block" />
            <span className="text-[var(--color-primary-600)] dark:text-sky-400">simplified.</span>
          </motion.h1>
          
          <motion.p 
            variants={FADE_UP_ANIMATION_VARIANTS}
            className="text-xl text-gray-600 dark:text-gray-300 mb-10 max-w-2xl mx-auto"
          >
            Find the right doctor, book appointments seamlessly, and manage your health records in one secure platform.
          </motion.p>
          
          <motion.div 
            variants={FADE_UP_ANIMATION_VARIANTS}
            className="flex flex-col sm:flex-row items-center justify-center gap-4 max-w-2xl mx-auto"
          >
            <Link href="/request-demo">
              <Button size="lg" className="w-full sm:w-auto h-14 px-12 text-lg shadow-md shadow-blue-500/20">
                Request a Demo
              </Button>
            </Link>
          </motion.div>
        </motion.div>
      </section>

      {/* Features Section */}
      <section className="w-full py-24 bg-white dark:bg-slate-900">
        <div className="container mx-auto px-4">
          <div className="text-center mb-16">
            <h2 className="text-3xl font-bold mb-4">Everything you need for better care</h2>
            <p className="text-gray-500 dark:text-gray-400 max-w-2xl mx-auto">
              We've redesigned the patient experience from the ground up to eliminate friction and put your health first.
            </p>
          </div>
          
          <div className="grid md:grid-cols-3 gap-8 max-w-5xl mx-auto">
            <Card className="border-0 shadow-lg shadow-gray-200/50 dark:shadow-none bg-blue-50/50 dark:bg-slate-800/50 hover:-translate-y-1 transition-transform duration-300">
              <CardHeader>
                <div className="h-12 w-12 rounded-xl bg-[var(--color-primary-600)] text-white flex items-center justify-center mb-4">
                  <Search className="h-6 w-6" />
                </div>
                <CardTitle>Intelligent Discovery</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base">
                  Search by symptoms or health concerns and we'll connect you with the most relevant specialists automatically.
                </CardDescription>
              </CardContent>
            </Card>
            
            <Card className="border-0 shadow-lg shadow-gray-200/50 dark:shadow-none bg-emerald-50/50 dark:bg-slate-800/50 hover:-translate-y-1 transition-transform duration-300">
              <CardHeader>
                <div className="h-12 w-12 rounded-xl bg-[var(--color-secondary-500)] text-white flex items-center justify-center mb-4">
                  <CalendarCheck className="h-6 w-6" />
                </div>
                <CardTitle>Instant Booking</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base">
                  See real-time availability and book your appointment instantly without making a single phone call.
                </CardDescription>
              </CardContent>
            </Card>
            
            <Card className="border-0 shadow-lg shadow-gray-200/50 dark:shadow-none bg-purple-50/50 dark:bg-slate-800/50 hover:-translate-y-1 transition-transform duration-300">
              <CardHeader>
                <div className="h-12 w-12 rounded-xl bg-purple-600 text-white flex items-center justify-center mb-4">
                  <FileText className="h-6 w-6" />
                </div>
                <CardTitle>Digital Prescriptions</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription className="text-base">
                  Access your consultation notes, prescriptions, and complete medical history securely from anywhere.
                </CardDescription>
              </CardContent>
            </Card>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section id="join" className="w-full py-24 bg-[var(--color-primary-600)] text-white relative overflow-hidden">
        <div className="absolute top-0 right-0 p-32 opacity-10">
          <Activity className="w-96 h-96" />
        </div>
        <div className="container mx-auto px-4 relative z-10 text-center">
          <h2 className="text-4xl font-bold mb-6">Ready to prioritize your health?</h2>
          <p className="text-blue-100 mb-10 max-w-2xl mx-auto text-lg">
            Join thousands of patients who have already switched to MedicHp for a better healthcare experience.
          </p>
          <div className="flex justify-center gap-4">
            <Link href="/register">
              <Button size="lg" className="bg-white text-[var(--color-primary-600)] hover:bg-gray-100 hover:text-[var(--color-primary-600)]">
                Create Patient Account
              </Button>
            </Link>
            <Link href="/request-demo">
              <Button size="lg" variant="outline" className="border-white text-white hover:bg-white/10 dark:hover:bg-white/10">
                Request a Demo
              </Button>
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
}
