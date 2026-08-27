"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Star, MapPin, Clock, Award, GraduationCap, Video, Users } from "lucide-react";
import Link from "next/link";
import { useParams } from "next/navigation";

export default function DoctorProfilePublic() {
  const params = useParams();
  
  return (
    <div className="container mx-auto px-4 py-8 lg:py-12 pb-24 md:pb-12 max-w-5xl">
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        {/* Left Column: Core Info */}
        <div className="lg:col-span-2 space-y-6">
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }}>
            <Card>
              <CardContent className="p-6 md:p-8">
                <div className="flex flex-col md:flex-row gap-6 items-start">
                  <div className="w-24 h-24 rounded-2xl bg-sky-100 flex items-center justify-center text-[var(--color-primary-600)] text-3xl font-bold shrink-0">
                    S
                  </div>
                  <div className="flex-1">
                    <div className="flex flex-col md:flex-row md:items-center justify-between gap-2 mb-2">
                      <h1 className="text-2xl md:text-3xl font-bold text-gray-900 dark:text-white">Dr. Sarah Jenkins</h1>
                      <div className="flex items-center gap-1 bg-amber-100 text-amber-700 px-2.5 py-1 rounded-md text-sm font-semibold w-fit">
                        <Star className="w-4 h-4 fill-amber-500 text-amber-500" />
                        4.9 (124 reviews)
                      </div>
                    </div>
                    <p className="text-lg text-[var(--color-primary-600)] font-medium mb-4">Cardiology Specialist</p>
                    <p className="text-gray-600 dark:text-gray-400 mb-4 leading-relaxed">
                      Dr. Jenkins has over 15 years of experience treating complex cardiovascular conditions. She is committed to providing comprehensive, patient-centered care.
                    </p>
                    <div className="flex flex-wrap gap-4 text-sm text-gray-500">
                      <span className="flex items-center gap-1.5"><GraduationCap className="w-4 h-4"/> MD, Harvard Medical</span>
                      <span className="flex items-center gap-1.5"><Award className="w-4 h-4"/> Board Certified</span>
                      <span className="flex items-center gap-1.5"><Users className="w-4 h-4"/> 5,000+ Patients</span>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          </motion.div>

          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }}>
            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-4">Experience & Background</h2>
            <Card>
              <CardContent className="p-6 space-y-4 text-gray-600 dark:text-gray-400">
                <p>Specializes in preventive cardiology, heart failure, and echocardiography.</p>
                <p>Previously served as the Head of Cardiology at St. Jude's Medical Center.</p>
              </CardContent>
            </Card>
          </motion.div>
        </div>

        {/* Right Column: Booking Widget */}
        <div className="space-y-6">
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }} className="sticky top-24">
            <Card className="border-[var(--color-primary-600)] shadow-xl shadow-sky-900/5">
              <CardContent className="p-6">
                <h3 className="font-bold text-lg text-gray-900 dark:text-white mb-4">Book Appointment</h3>
                
                <div className="space-y-4 mb-6">
                  <div className="flex items-start gap-3">
                    <Video className="w-5 h-5 text-[var(--color-secondary-500)] mt-0.5 shrink-0" />
                    <div>
                      <p className="font-medium text-gray-900 dark:text-white">Video Consultation</p>
                      <p className="text-sm text-gray-500">Available Today</p>
                      <p className="text-sm font-semibold text-[var(--color-primary-600)] mt-1">$150 / session</p>
                    </div>
                  </div>
                  
                  <div className="flex items-start gap-3">
                    <MapPin className="w-5 h-5 text-[var(--color-primary-600)] mt-0.5 shrink-0" />
                    <div>
                      <p className="font-medium text-gray-900 dark:text-white">In-Person Visit</p>
                      <p className="text-sm text-gray-500">123 Health Ave, NY</p>
                      <p className="text-sm font-semibold text-[var(--color-primary-600)] mt-1">$200 / visit</p>
                    </div>
                  </div>
                </div>

                <Link href={`/book/${params.id}`}>
                  <Button className="w-full" size="lg">Book Now</Button>
                </Link>
                <p className="text-xs text-center text-gray-400 mt-3">Free cancellation up to 24 hours prior.</p>
              </CardContent>
            </Card>
          </motion.div>
        </div>
        
      </div>
    </div>
  );
}
