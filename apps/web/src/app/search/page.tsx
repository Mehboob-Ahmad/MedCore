"use client";

import { useState } from "react";
import { motion } from "framer-motion";
import { Search, MapPin, Star, Clock } from "lucide-react";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Card, CardContent } from "@medichp/ui";

// Mock data for Phase 1 UI showcase
const MOCK_DOCTORS = [
  {
    id: 1,
    name: "Dr. Ayesha Khan",
    specialty: "Dermatologist",
    experience: "12 years exp.",
    rating: 4.9,
    reviews: 124,
    fee: "₹800",
    location: "Lahore Medical Center",
    nextAvailable: "Today, 4:00 PM",
  },
  {
    id: 2,
    name: "Dr. Ahmed Ali",
    specialty: "Cardiologist",
    experience: "15 years exp.",
    rating: 4.8,
    reviews: 89,
    fee: "₹1200",
    location: "Heart Care Clinic",
    nextAvailable: "Tomorrow, 10:30 AM",
  },
  {
    id: 3,
    name: "Dr. Fatima Hassan",
    specialty: "Pediatrician",
    experience: "8 years exp.",
    rating: 4.7,
    reviews: 210,
    fee: "₹600",
    location: "Kids Health Clinic",
    nextAvailable: "Today, 5:15 PM",
  }
];

export default function SearchPage() {
  const [query, setQuery] = useState("");

  return (
    <div className="bg-surface-50 dark:bg-slate-900 min-h-screen pb-20">
      {/* Search Header */}
      <div className="bg-white dark:bg-slate-800 border-b border-gray-200 dark:border-slate-700 py-12">
        <div className="container mx-auto px-4 max-w-5xl">
          <h1 className="text-3xl md:text-4xl font-bold mb-6 text-gray-900 dark:text-white">
            Find the right doctor for you
          </h1>
          <div className="flex flex-col md:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <Input 
                placeholder="Symptoms, specialties, or doctor name..." 
                className="pl-10 h-14"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
              />
            </div>
            <div className="relative md:w-64">
              <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <Input 
                placeholder="City or location" 
                className="pl-10 h-14"
              />
            </div>
            <Button size="lg" className="h-14 px-8">Search</Button>
          </div>
          
          <div className="flex gap-2 mt-6 overflow-x-auto pb-2 scrollbar-hide">
            {["Dermatology", "Cardiology", "Pediatrics", "Neurology", "Orthopedics"].map((spec) => (
              <button 
                key={spec} 
                className="px-4 py-2 rounded-full border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-sm font-medium hover:border-[var(--color-primary-600)] hover:text-[var(--color-primary-600)] transition-colors whitespace-nowrap"
              >
                {spec}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Results Section */}
      <div className="container mx-auto px-4 max-w-5xl mt-10 flex flex-col md:flex-row gap-8">
        {/* Filters Sidebar */}
        <div className="w-full md:w-64 shrink-0 space-y-6 hidden md:block">
          <div>
            <h3 className="font-semibold text-lg mb-4">Filters</h3>
            <div className="space-y-4 border-t border-gray-200 dark:border-slate-700 pt-4">
              <h4 className="font-medium text-sm text-gray-900 dark:text-gray-100">Consultation Fee</h4>
              <div className="space-y-2">
                {["Under ₹500", "₹500 - ₹1000", "Above ₹1000"].map((fee) => (
                  <label key={fee} className="flex items-center gap-2 cursor-pointer">
                    <input type="checkbox" className="rounded border-gray-300 text-[var(--color-primary-600)] focus:ring-[var(--color-primary-600)]" />
                    <span className="text-sm text-gray-600 dark:text-gray-400">{fee}</span>
                  </label>
                ))}
              </div>
            </div>
            <div className="space-y-4 border-t border-gray-200 dark:border-slate-700 pt-4 mt-4">
              <h4 className="font-medium text-sm text-gray-900 dark:text-gray-100">Availability</h4>
              <div className="space-y-2">
                {["Available Today", "Available Tomorrow", "Next 3 Days"].map((time) => (
                  <label key={time} className="flex items-center gap-2 cursor-pointer">
                    <input type="checkbox" className="rounded border-gray-300 text-[var(--color-primary-600)] focus:ring-[var(--color-primary-600)]" />
                    <span className="text-sm text-gray-600 dark:text-gray-400">{time}</span>
                  </label>
                ))}
              </div>
            </div>
          </div>
        </div>

        {/* Doctor List */}
        <div className="flex-1 space-y-6">
          <div className="flex justify-between items-center mb-4">
            <span className="text-gray-600 dark:text-gray-400 font-medium">3 doctors found</span>
            <select className="border-gray-300 dark:border-slate-700 bg-white dark:bg-slate-800 rounded-lg text-sm font-medium py-2 px-3 focus:ring-[var(--color-primary-600)] focus:border-[var(--color-primary-600)]">
              <option>Sort by Relevance</option>
              <option>Sort by Fee: Low to High</option>
              <option>Sort by Rating</option>
            </select>
          </div>

          <motion.div 
            initial="hidden"
            animate="show"
            variants={{
              hidden: {},
              show: { transition: { staggerChildren: 0.1 } }
            }}
            className="space-y-4"
          >
            {MOCK_DOCTORS.map((doc) => (
              <motion.div
                key={doc.id}
                variants={{
                  hidden: { opacity: 0, y: 20 },
                  show: { opacity: 1, y: 0 }
                }}
              >
                <Card className="overflow-hidden hover:border-[var(--color-primary-600)] transition-colors">
                  <CardContent className="p-0 sm:flex">
                    <div className="p-6 sm:w-2/3 border-b sm:border-b-0 sm:border-r border-gray-100 dark:border-slate-800">
                      <div className="flex gap-4">
                        <div className="w-16 h-16 rounded-full bg-blue-100 dark:bg-slate-700 flex items-center justify-center text-xl font-bold text-[var(--color-primary-600)] shrink-0">
                          {doc.name.charAt(4)}
                        </div>
                        <div>
                          <h3 className="font-bold text-lg text-[var(--color-primary-600)] dark:text-sky-400">{doc.name}</h3>
                          <p className="text-sm font-medium text-gray-900 dark:text-white">{doc.specialty}</p>
                          <p className="text-sm text-gray-500 mt-1">{doc.experience}</p>
                          
                          <div className="flex items-center gap-1 mt-3">
                            <Star className="w-4 h-4 fill-[var(--color-warning-500)] text-[var(--color-warning-500)]" />
                            <span className="font-medium text-sm">{doc.rating}</span>
                            <span className="text-xs text-gray-500">({doc.reviews} reviews)</span>
                          </div>
                          
                          <div className="flex items-center gap-1 mt-2 text-sm text-gray-600 dark:text-gray-400">
                            <MapPin className="w-4 h-4" />
                            {doc.location}
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    <div className="p-6 sm:w-1/3 bg-gray-50 dark:bg-slate-800/50 flex flex-col justify-center">
                      <div className="flex items-center gap-2 mb-2 text-sm text-[var(--color-success-600)] font-medium">
                        <Clock className="w-4 h-4" />
                        {doc.nextAvailable}
                      </div>
                      <div className="text-sm text-gray-500 dark:text-gray-400 mb-4">
                        Consultation Fee: <span className="font-bold text-gray-900 dark:text-white text-base">{doc.fee}</span>
                      </div>
                      <Button className="w-full">Book Appointment</Button>
                    </div>
                  </CardContent>
                </Card>
              </motion.div>
            ))}
          </motion.div>
        </div>
      </div>
    </div>
  );
}
