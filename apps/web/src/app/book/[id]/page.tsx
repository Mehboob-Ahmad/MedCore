"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Calendar as CalendarIcon, Clock, Video, User, CheckCircle } from "lucide-react";
import Link from "next/link";
import { useState } from "react";
import { useParams } from "next/navigation";

export default function BookingFlow() {
  const [step, setStep] = useState(1);
  const [selectedDate, setSelectedDate] = useState<number | null>(null);
  const [selectedTime, setSelectedTime] = useState<string | null>(null);
  const params = useParams();

  const dates = [
    { day: "Mon", date: 12 },
    { day: "Tue", date: 13 },
    { day: "Wed", date: 14 },
    { day: "Thu", date: 15 },
    { day: "Fri", date: 16 },
  ];

  const times = ["09:00 AM", "10:30 AM", "01:00 PM", "03:30 PM", "04:00 PM"];

  return (
    <div className="container mx-auto px-4 py-8 lg:py-12 pb-24 max-w-3xl">
      <div className="mb-8 text-center">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-2">Book Appointment</h1>
        <p className="text-gray-500">Dr. Sarah Jenkins - Cardiology</p>
      </div>

      <div className="flex justify-center mb-12">
        <div className="flex items-center gap-2">
          <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold text-sm ${step >= 1 ? 'bg-[var(--color-primary-600)] text-white' : 'bg-gray-200 text-gray-500'}`}>1</div>
          <div className={`w-12 h-1 rounded-full ${step >= 2 ? 'bg-[var(--color-primary-600)]' : 'bg-gray-200'}`}></div>
          <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold text-sm ${step >= 2 ? 'bg-[var(--color-primary-600)] text-white' : 'bg-gray-200 text-gray-500'}`}>2</div>
          <div className={`w-12 h-1 rounded-full ${step >= 3 ? 'bg-[var(--color-primary-600)]' : 'bg-gray-200'}`}></div>
          <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold text-sm ${step >= 3 ? 'bg-[var(--color-primary-600)] text-white' : 'bg-gray-200 text-gray-500'}`}>3</div>
        </div>
      </div>

      <Card>
        <CardContent className="p-6 md:p-8">
          
          {step === 1 && (
            <motion.div initial={{ opacity: 0, x: 20 }} animate={{ opacity: 1, x: 0 }} className="space-y-6">
              <h2 className="text-xl font-bold text-gray-900 dark:text-white flex items-center gap-2">
                <CalendarIcon className="w-5 h-5 text-[var(--color-primary-600)]" /> Select Date & Time
              </h2>
              
              <div>
                <p className="font-medium text-sm text-gray-700 dark:text-gray-300 mb-3">October 2026</p>
                <div className="flex gap-3 overflow-x-auto pb-2">
                  {dates.map((d, i) => (
                    <button 
                      key={i}
                      onClick={() => setSelectedDate(i)}
                      className={`min-w-[70px] p-3 rounded-xl border flex flex-col items-center justify-center transition-all ${selectedDate === i ? 'border-[var(--color-primary-600)] bg-sky-50 dark:bg-sky-900/20 text-[var(--color-primary-600)]' : 'border-gray-200 dark:border-slate-700 hover:border-sky-300 text-gray-600 dark:text-gray-400'}`}
                    >
                      <span className="text-xs uppercase font-medium">{d.day}</span>
                      <span className="text-xl font-bold">{d.date}</span>
                    </button>
                  ))}
                </div>
              </div>

              {selectedDate !== null && (
                <motion.div initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }}>
                  <p className="font-medium text-sm text-gray-700 dark:text-gray-300 mb-3 mt-6">Available Slots</p>
                  <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
                    {times.map((t, i) => (
                      <button 
                        key={i}
                        onClick={() => setSelectedTime(t)}
                        className={`p-3 rounded-xl border text-sm font-medium transition-all flex items-center justify-center gap-2 ${selectedTime === t ? 'border-[var(--color-primary-600)] bg-sky-50 dark:bg-sky-900/20 text-[var(--color-primary-600)]' : 'border-gray-200 dark:border-slate-700 hover:border-sky-300 text-gray-600 dark:text-gray-400'}`}
                      >
                        <Clock className="w-4 h-4" /> {t}
                      </button>
                    ))}
                  </div>
                </motion.div>
              )}

              <div className="pt-6 border-t border-gray-100 dark:border-slate-800 flex justify-end">
                <Button disabled={!selectedDate || !selectedTime} onClick={() => setStep(2)}>Continue</Button>
              </div>
            </motion.div>
          )}

          {step === 2 && (
            <motion.div initial={{ opacity: 0, x: 20 }} animate={{ opacity: 1, x: 0 }} className="space-y-6">
              <h2 className="text-xl font-bold text-gray-900 dark:text-white">Consultation Details</h2>
              
              <div className="space-y-4">
                <div className="p-4 border border-gray-200 dark:border-slate-700 rounded-xl bg-slate-50 dark:bg-slate-800/50">
                  <div className="flex items-start gap-4">
                    <Video className="w-6 h-6 text-[var(--color-primary-600)] shrink-0" />
                    <div>
                      <h3 className="font-bold text-gray-900 dark:text-white">Video Consultation</h3>
                      <p className="text-sm text-gray-500 mt-1">Join the call from your device anywhere. A secure link will be provided.</p>
                    </div>
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Reason for visit</label>
                  <textarea className="w-full min-h-[100px] p-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-gray-900 dark:text-white text-sm focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-600)] focus:border-transparent transition-shadow" placeholder="Briefly describe your symptoms or reason for the consultation..." />
                </div>
              </div>

              <div className="pt-6 border-t border-gray-100 dark:border-slate-800 flex justify-between">
                <Button variant="outline" onClick={() => setStep(1)}>Back</Button>
                <Button onClick={() => setStep(3)}>Confirm & Pay $150</Button>
              </div>
            </motion.div>
          )}

          {step === 3 && (
            <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="text-center py-8 space-y-6">
              <div className="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
                <CheckCircle className="w-10 h-10 text-green-600" />
              </div>
              <h2 className="text-2xl font-bold text-gray-900 dark:text-white">Booking Confirmed!</h2>
              <p className="text-gray-500 max-w-md mx-auto">
                Your appointment with Dr. Sarah Jenkins is confirmed for Oct 12, 2026 at {selectedTime}. We've sent a calendar invite to your email.
              </p>
              
              <div className="pt-6">
                <Link href="/patient/appointments">
                  <Button size="lg">Go to My Appointments</Button>
                </Link>
              </div>
            </motion.div>
          )}

        </CardContent>
      </Card>
    </div>
  );
}
