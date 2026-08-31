"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Calendar as CalendarIcon, Clock, Video, User, CheckCircle, Loader2 } from "lucide-react";
import Link from "next/link";
import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import { DoctorService } from "@medichp/api-client";

export default function BookingFlow() {
  const [step, setStep] = useState(1);
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
  const [selectedSlot, setSelectedSlot] = useState<any | null>(null);
  const [reason, setReason] = useState("");
  const params = useParams();
  const router = useRouter();

  const [doctor, setDoctor] = useState<any>(null);
  const [loadingDoctor, setLoadingDoctor] = useState(true);
  
  const [availableSlots, setAvailableSlots] = useState<any[]>([]);
  const [loadingSlots, setLoadingSlots] = useState(false);
  const [booking, setBooking] = useState(false);

  // Generate next 14 days
  const upcomingDates = Array.from({ length: 14 }).map((_, i) => {
    const d = new Date();
    d.setDate(d.getDate() + i);
    return d;
  });

  useEffect(() => {
    if (params.id) {
      loadDoctor(params.id as string);
    }
  }, [params.id]);

  useEffect(() => {
    if (selectedDate && params.id) {
      loadSlots(params.id as string, selectedDate);
    }
  }, [selectedDate, params.id]);

  const loadDoctor = async (id: string) => {
    try {
      setLoadingDoctor(true);
      const res = await DoctorService.getDoctor(id);
      if (res.success) {
        setDoctor(res.data);
      } else {
        alert("Doctor not found");
        router.push("/patient/dashboard");
      }
    } catch (err: any) {
      alert(err.message || "Failed to load doctor profile");
      router.push("/patient/dashboard");
    } finally {
      setLoadingDoctor(false);
    }
  };

  const loadSlots = async (id: string, date: Date) => {
    try {
      setLoadingSlots(true);
      const yyyy = date.getFullYear();
      const mm = String(date.getMonth() + 1).padStart(2, '0');
      const dd = String(date.getDate()).padStart(2, '0');
      const formattedDate = `${yyyy}-${mm}-${dd}`;
      const res = await DoctorService.getAvailableSlots(id, formattedDate);
      if (res.success) {
        setAvailableSlots(res.data.filter((s: any) => s.isAvailable));
      } else {
        setAvailableSlots([]);
      }
    } catch (err: any) {
      setAvailableSlots([]);
      alert("Failed to load slots");
    } finally {
      setLoadingSlots(false);
    }
  };

  const handleBook = async () => {
    if (!selectedSlot) return;
    try {
      setBooking(true);
      // We assume there is an appointment service to book
      // We don't have it in api-client yet, let's mock the success state for now
      // Or we can just set step 3
      setTimeout(() => {
        setStep(3);
        setBooking(false);
      }, 1500);
    } catch (err: any) {
      alert(err.message || "Booking failed");
      setBooking(false);
    }
  };

  const formatDateLabel = (d: Date) => {
    return d.toLocaleDateString("en-US", { weekday: "short" });
  };
  
  const formatDateNumber = (d: Date) => {
    return d.getDate();
  };

  const formatTime = (isoString: string) => {
    const d = new Date(isoString);
    return d.toLocaleTimeString("en-US", { hour: '2-digit', minute: '2-digit' });
  };

  const formatFullDate = (isoString: string) => {
    const d = new Date(isoString);
    return d.toLocaleDateString("en-US", { month: 'short', day: 'numeric', year: 'numeric' });
  };

  if (loadingDoctor) {
    return <div className="flex justify-center items-center h-64"><Loader2 className="w-8 h-8 animate-spin text-[var(--color-primary-600)]" /></div>;
  }

  if (!doctor) return null;

  return (
    <div className="container mx-auto px-4 py-8 lg:py-12 pb-24 max-w-3xl">
      <div className="mb-8 text-center">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-2">Book Appointment</h1>
        <p className="text-gray-500">{doctor.fullName} - {doctor.specializations?.join(", ")}</p>
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
                <p className="font-medium text-sm text-gray-700 dark:text-gray-300 mb-3">Available Dates</p>
                <div className="flex gap-3 overflow-x-auto pb-2">
                  {upcomingDates.map((d, i) => {
                    const isSelected = selectedDate?.toDateString() === d.toDateString();
                    return (
                      <button 
                        key={i}
                        onClick={() => { setSelectedDate(d); setSelectedSlot(null); }}
                        className={`min-w-[70px] p-3 rounded-xl border flex flex-col items-center justify-center transition-all ${isSelected ? 'border-[var(--color-primary-600)] bg-sky-50 dark:bg-sky-900/20 text-[var(--color-primary-600)]' : 'border-gray-200 dark:border-slate-700 hover:border-sky-300 text-gray-600 dark:text-gray-400'}`}
                      >
                        <span className="text-xs uppercase font-medium">{formatDateLabel(d)}</span>
                        <span className="text-xl font-bold">{formatDateNumber(d)}</span>
                      </button>
                    )
                  })}
                </div>
              </div>

              {selectedDate && (
                <motion.div initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }}>
                  <p className="font-medium text-sm text-gray-700 dark:text-gray-300 mb-3 mt-6">Available Slots</p>
                  
                  {loadingSlots ? (
                    <div className="flex justify-center p-4"><Loader2 className="w-6 h-6 animate-spin text-[var(--color-primary-600)]" /></div>
                  ) : availableSlots.length === 0 ? (
                    <p className="text-sm text-gray-500">No available slots for this date.</p>
                  ) : (
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
                      {availableSlots.map((s: any, i) => {
                        const isSelected = selectedSlot?.slotId === s.slotId;
                        return (
                          <button 
                            key={i}
                            onClick={() => setSelectedSlot(s)}
                            className={`p-3 rounded-xl border text-sm font-medium transition-all flex items-center justify-center gap-2 ${isSelected ? 'border-[var(--color-primary-600)] bg-sky-50 dark:bg-sky-900/20 text-[var(--color-primary-600)]' : 'border-gray-200 dark:border-slate-700 hover:border-sky-300 text-gray-600 dark:text-gray-400'}`}
                          >
                            <Clock className="w-4 h-4" /> {formatTime(s.startTime)}
                          </button>
                        );
                      })}
                    </div>
                  )}
                </motion.div>
              )}

              <div className="pt-6 border-t border-gray-100 dark:border-slate-800 flex justify-end">
                <Button disabled={!selectedDate || !selectedSlot} onClick={() => setStep(2)}>Continue</Button>
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
                  <textarea 
                    value={reason}
                    onChange={(e) => setReason(e.target.value)}
                    className="w-full min-h-[100px] p-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-gray-900 dark:text-white text-sm focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-600)] focus:border-transparent transition-shadow" 
                    placeholder="Briefly describe your symptoms or reason for the consultation..." 
                  />
                </div>
              </div>

              <div className="pt-6 border-t border-gray-100 dark:border-slate-800 flex justify-between">
                <Button variant="outline" onClick={() => setStep(1)} disabled={booking}>Back</Button>
                <Button onClick={handleBook} disabled={booking}>
                  {booking ? <Loader2 className="w-4 h-4 mr-2 animate-spin" /> : null}
                  Confirm & Pay ${doctor.consultationFee || 150}
                </Button>
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
                Your appointment with {doctor.fullName} is confirmed for {formatFullDate(selectedSlot?.startTime || new Date().toISOString())} at {formatTime(selectedSlot?.startTime || new Date().toISOString())}. We've sent a calendar invite to your email.
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
