"use client";

import { motion } from "framer-motion";
import { UserCircle, BriefcaseMedical, Phone, Building2, Clock, DollarSign, FileBadge } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { useRouter } from "next/navigation";

export default function CompleteProfilePage() {
  const router = useRouter();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Navigate to dashboard after completion
    router.push("/doctor/dashboard");
  };

  return (
    <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-blue-100/40 via-transparent to-transparent dark:from-blue-900/10 py-12">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="w-full max-w-2xl"
      >
        <Card className="border-0 shadow-2xl shadow-blue-900/5 dark:shadow-none dark:bg-slate-800/80 border-t-4 border-t-[var(--color-primary-500)]">
          <CardHeader className="text-center pb-8 border-b border-gray-100 dark:border-gray-800">
            <div className="mx-auto w-16 h-16 bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 rounded-full flex items-center justify-center mb-4">
              <UserCircle size={32} />
            </div>
            <CardTitle className="text-2xl font-bold text-gray-900 dark:text-white">Complete Your Profile</CardTitle>
            <CardDescription className="text-gray-500 dark:text-gray-400 text-base mt-2">
              Tell us more about your practice to start accepting patients.
            </CardDescription>
          </CardHeader>
          <CardContent className="pt-8">
            <form className="space-y-6" onSubmit={handleSubmit}>
              
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Specialization</label>
                  <div className="relative">
                    <BriefcaseMedical className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input placeholder="e.g. Cardiology" className="pl-10" required />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Contact Number</label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input type="tel" placeholder="+1 (555) 000-0000" className="pl-10" required />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Licensing Authority</label>
                  <div className="relative">
                    <FileBadge className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input placeholder="e.g. Medical Council" className="pl-10" required />
                  </div>
                </div>
                
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Clinic / Hospital Name</label>
                  <div className="relative">
                    <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input placeholder="City General Hospital" className="pl-10" required />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Availability Hours</label>
                  <div className="relative">
                    <Clock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input placeholder="Mon-Fri, 9:00 AM - 5:00 PM" className="pl-10" required />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Consultation Fee ($)</label>
                  <div className="relative">
                    <DollarSign className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input type="number" placeholder="100" className="pl-10" required />
                  </div>
                </div>
              </div>

              <div className="pt-4 border-t border-gray-100 dark:border-gray-800 flex justify-end">
                <Button type="button" variant="outline" className="mr-3" onClick={() => router.push('/doctor/dashboard')}>
                  Skip for now
                </Button>
                <Button type="submit" className="bg-[var(--color-primary-600)] hover:bg-[var(--color-primary-700)] text-white px-8">
                  Save Profile
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}
