"use client";

import Link from "next/link";
import { motion } from "framer-motion";
import { Stethoscope, User, Mail, Lock, Upload } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardFooter } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { useRouter } from "next/navigation";

export default function DoctorRegisterPage() {
  const router = useRouter();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Navigate to complete profile after registration
    router.push("/doctor/complete-profile");
  };

  return (
    <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-blue-100/40 via-transparent to-transparent dark:from-blue-900/10">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="w-full max-w-md my-12"
      >
        <div className="text-center mb-8">
          <Link href="/" className="inline-flex items-center space-x-2">
            <Stethoscope className="w-8 h-8 text-[var(--color-primary-600)]" />
            <span className="font-bold text-2xl tracking-tight text-[var(--color-primary-600)] dark:text-sky-400">
              MedicHp
            </span>
          </Link>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Join MedicHp as a Doctor</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2">Upload your documents to get verified.</p>
        </div>

        <Card className="border-0 shadow-xl shadow-blue-900/5 dark:shadow-none dark:bg-slate-800/80 border-t-4 border-t-[var(--color-secondary-500)]">
          <CardContent className="pt-6">
            <form className="space-y-4" onSubmit={handleSubmit}>
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Full Name</label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input placeholder="Dr. Jane Smith" className="pl-10" required />
                </div>
              </div>
              
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email address</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input type="email" placeholder="jane@clinic.com" className="pl-10" required />
                </div>
              </div>
              
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input type="password" placeholder="••••••••" className="pl-10" required />
                </div>
              </div>

              <div className="pt-2">
                <h3 className="text-sm font-semibold text-gray-800 dark:text-gray-200 mb-3 border-b border-gray-100 dark:border-gray-700 pb-2">Verification Documents</h3>
                
                <div className="space-y-4">
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Upload MBBS Degree</label>
                    <div className="relative flex items-center">
                      <Input type="file" accept="image/*,.pdf" className="pl-10 py-1.5" required />
                      <Upload className="absolute left-3 h-4 w-4 text-gray-400" />
                    </div>
                    <p className="text-xs text-gray-500">Image or PDF format</p>
                  </div>
                  
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Upload Doctor's License</label>
                    <div className="relative flex items-center">
                      <Input type="file" accept="image/*,.pdf" className="pl-10 py-1.5" required />
                      <Upload className="absolute left-3 h-4 w-4 text-gray-400" />
                    </div>
                    <p className="text-xs text-gray-500">Image or PDF format</p>
                  </div>
                </div>
              </div>

              <Button type="submit" className="w-full mt-6 bg-slate-900 hover:bg-slate-800 dark:bg-slate-700 dark:hover:bg-slate-600" size="lg">
                Register & Continue
              </Button>
            </form>
          </CardContent>
          <CardFooter className="flex justify-center border-t border-gray-100 dark:border-slate-700/50 pt-6">
            <p className="text-sm text-gray-600 dark:text-gray-400">
              Already have an account?{" "}
              <Link href="/login" className="font-semibold text-[var(--color-primary-600)] hover:underline">
                Log in
              </Link>
            </p>
          </CardFooter>
        </Card>
      </motion.div>
    </div>
  );
}
