"use client";

import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { Search, MapPin, Star, Clock } from "lucide-react";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Card, CardContent } from "@medichp/ui";
import { DoctorService } from "@medichp/api-client";
import Link from "next/link";

export default function SearchPage() {
  const [query, setQuery] = useState("");
  const [specialty, setSpecialty] = useState("");
  const [doctors, setDoctors] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [cities, setCities] = useState<any[]>([]);
  const [selectedCityIds, setSelectedCityIds] = useState<string[]>([]);

  const fetchDoctors = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await DoctorService.searchDoctors(query, specialty, undefined, selectedCityIds);
      if (res.success) {
        // Data format differs depending on backend pagination implementation, check for data.doctors or data directly
        setDoctors(res.data.doctors || res.data || []);
      } else {
        setError("Failed to fetch doctors");
      }
    } catch (err: any) {
      setError(err.message || "Error fetching doctors");
    } finally {
      setLoading(false);
    }
  };

  const fetchCities = async () => {
    try {
      const res = await SystemService.getCities();
      if (res.success) {
        setCities(res.data || []);
      }
    } catch (err) {
      console.error("Failed to fetch cities", err);
    }
  };

  useEffect(() => {
    fetchCities();
  }, []);

  useEffect(() => {
    fetchDoctors();
  }, [specialty, selectedCityIds]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    fetchDoctors();
  };

  return (
    <div className="bg-surface-50 dark:bg-slate-900 min-h-screen pb-20">
      {/* Search Header */}
      <div className="bg-white dark:bg-slate-800 border-b border-gray-200 dark:border-slate-700 py-12">
        <div className="container mx-auto px-4 max-w-5xl">
          <h1 className="text-3xl md:text-4xl font-bold mb-6 text-gray-900 dark:text-white">
            Find the right doctor for you
          </h1>
          <form onSubmit={handleSearch} className="flex flex-col md:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <Input 
                placeholder="Doctor name..." 
                className="pl-10 h-14"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
              />
            </div>
            <div className="relative md:w-64">
              <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <select 
                multiple
                className="pl-10 h-24 w-full border-gray-300 dark:border-slate-700 bg-white dark:bg-slate-800 rounded-lg text-sm text-gray-700 dark:text-gray-200 focus:ring-[var(--color-primary-600)] focus:border-[var(--color-primary-600)]"
                value={selectedCityIds}
                onChange={(e) => {
                  const options = e.target.options;
                  const selected = [];
                  for (let i = 0; i < options.length; i++) {
                    if (options[i].selected && options[i].value !== "") {
                      selected.push(options[i].value);
                    }
                  }
                  setSelectedCityIds(selected);
                }}
              >
                <option value="">All Cities (Click to deselect)</option>
                {cities.map((city) => (
                  <option key={city.id} value={city.id}>{city.name}</option>
                ))}
              </select>
            </div>
            <Button type="submit" size="lg" className="h-14 px-8">Search</Button>
          </form>
          
          <div className="flex gap-2 mt-6 overflow-x-auto pb-2 scrollbar-hide">
            {["Dermatology", "Cardiology", "Pediatrics", "Neurology", "Orthopedics"].map((spec) => (
              <button 
                key={spec} 
                onClick={() => setSpecialty(specialty === spec ? "" : spec)}
                className={`px-4 py-2 rounded-full border text-sm font-medium transition-colors whitespace-nowrap ${
                  specialty === spec 
                    ? "bg-[var(--color-primary-600)] border-[var(--color-primary-600)] text-white" 
                    : "border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-gray-700 dark:text-gray-300 hover:border-[var(--color-primary-600)] hover:text-[var(--color-primary-600)]"
                }`}
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
                {["Under PKR 1,000", "PKR 1,000 - PKR 3,000", "Above PKR 3,000"].map((fee) => (
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
            <span className="text-gray-600 dark:text-gray-400 font-medium">
              {loading ? "Searching..." : `${doctors.length} doctors found`}
            </span>
            <select className="border-gray-300 dark:border-slate-700 bg-white dark:bg-slate-800 rounded-lg text-sm font-medium py-2 px-3 focus:ring-[var(--color-primary-600)] focus:border-[var(--color-primary-600)]">
              <option>Sort by Relevance</option>
              <option>Sort by Fee: Low to High</option>
              <option>Sort by Rating</option>
            </select>
          </div>

          {error && (
            <div className="p-4 bg-red-50 text-red-600 border border-red-200 rounded-lg">
              {error}
            </div>
          )}

          {!loading && doctors.length === 0 && !error && (
            <div className="text-center py-12 text-gray-500 dark:text-gray-400">
              No doctors found matching your criteria. Try adjusting your search.
            </div>
          )}

          <motion.div 
            initial="hidden"
            animate="show"
            variants={{
              hidden: {},
              show: { transition: { staggerChildren: 0.1 } }
            }}
            className="space-y-4"
          >
            {doctors.map((doc) => (
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
                          {doc.user?.firstName?.charAt(0) || "D"}
                        </div>
                        <div>
                          <h3 className="font-bold text-lg text-[var(--color-primary-600)] dark:text-sky-400">
                            Dr. {doc.user?.firstName} {doc.user?.lastName}
                          </h3>
                          <p className="text-sm font-medium text-gray-900 dark:text-white">
                            {doc.specializations?.map((s: any) => s.specialization?.name).join(", ") || "General Physician"}
                          </p>
                          <p className="text-sm text-gray-500 mt-1">{doc.yearsOfExperience} years exp.</p>
                          
                          <div className="flex items-center gap-1 mt-3">
                            <Star className="w-4 h-4 fill-[var(--color-warning-500)] text-[var(--color-warning-500)]" />
                            <span className="font-medium text-sm">{(doc.averageRating || 0).toFixed(1)}</span>
                            <span className="text-xs text-gray-500">({doc.totalReviews || 0} reviews)</span>
                          </div>
                          
                          <div className="flex items-center gap-1 mt-2 text-sm text-gray-600 dark:text-gray-400">
                            <MapPin className="w-4 h-4" />
                            {doc.clinicName || doc.address || "Location not specified"} {doc.cityName ? `• ${doc.cityName}` : ""}
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    <div className="p-6 sm:w-1/3 bg-gray-50 dark:bg-slate-800/50 flex flex-col justify-center">
                      <div className="flex items-center gap-2 mb-2 text-sm text-[var(--color-success-600)] font-medium">
                        <Clock className="w-4 h-4" />
                        Next Available
                      </div>
                      <div className="text-sm text-gray-500 dark:text-gray-400 mb-4">
                        Consultation Fee: <span className="font-bold text-gray-900 dark:text-white text-base">PKR {doc.consultationFee || 0}</span>
                      </div>
                      <Link href={`/book/${doc.id}`} className="w-full">
                        <Button className="w-full">Book Appointment</Button>
                      </Link>
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
