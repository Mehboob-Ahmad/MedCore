"use client";

import { useState } from "react";
import Link from "next/link";
import { motion } from "framer-motion";
import { Stethoscope, User, Mail, Lock, Upload, Eye, EyeOff } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardFooter } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { useAuth } from "@/contexts/AuthContext";
import { AuthService } from "@medichp/api-client";

import { Logo } from "@/components/Logo";

export default function DoctorRegisterPage() {
  const { registerDoctor } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phoneNumber: "",
    password: "",
    confirmPassword: "",
    specialization: "",
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [degreeFile, setDegreeFile] = useState<File | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>, setFile: React.Dispatch<React.SetStateAction<File | null>>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    if (formData.password !== formData.confirmPassword) {
      setError("Passwords do not match");
      setLoading(false);
      return;
    }
    
    try {
      let degreeFileId = "00000000-0000-0000-0000-000000000000";
      let licenseFileId = "00000000-0000-0000-0000-000000000000";

      if (degreeFile) {
        const degreeRes = await AuthService.uploadFile(degreeFile, "Degree");
        if (degreeRes.success) degreeFileId = degreeRes.fileId;
      }

      const payload = {
        ...formData,
        confirmPassword: formData.confirmPassword,
        acceptTerms: true,
        mbbsDegreeFileId: degreeFileId,
        specialization: formData.specialization
      };

      await registerDoctor(payload);
      setSuccess(true);
    } catch (err: any) {
      setError(err.message || "An error occurred during registration");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-blue-100/40 via-transparent to-transparent dark:from-blue-900/10">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="w-full max-w-md my-12"
      >
        <div className="text-center mb-8 flex flex-col items-center">
          <Logo href="/" size={44} textSize="text-2xl" />
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Join MedicHp as a Doctor</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2">Upload your documents to get verified.</p>
        </div>

        <Card className="border-0 shadow-xl shadow-blue-900/5 dark:shadow-none dark:bg-slate-800/80 border-t-4 border-t-[var(--color-secondary-500)]">
          <CardContent className="pt-6">
            {success ? (
              <div className="text-center space-y-6 py-8">
                <div className="w-16 h-16 bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400 rounded-full flex items-center justify-center mx-auto">
                  <Stethoscope className="w-8 h-8" />
                </div>
                <div>
                  <h2 className="text-xl font-bold text-gray-900 dark:text-white">Doctor Account Created!</h2>
                  <p className="text-gray-500 dark:text-gray-400 mt-2 text-sm">
                    We sent a 4-digit verification code to <span className="font-semibold text-gray-700 dark:text-gray-300">{formData.email}</span>. Please verify your email to activate your account.
                  </p>
                </div>
                <div className="flex flex-col gap-3">
                  <Link href={`/verify-email?email=${encodeURIComponent(formData.email)}`} className="inline-block w-full">
                    <Button className="w-full bg-[var(--color-primary-600)] hover:bg-blue-700 text-white" size="lg">
                      Verify Email with Code &rarr;
                    </Button>
                  </Link>
                  <Link href="/login" className="inline-block w-full">
                    <Button variant="outline" className="w-full" size="sm">
                      Go to Login
                    </Button>
                  </Link>
                </div>
              </div>
            ) : (
              <>
                {error && (
                  <div className="mb-4 p-3 rounded-lg bg-red-50 text-red-600 text-sm border border-red-200 dark:bg-red-950/40 dark:text-red-400 dark:border-red-800">
                    {error}
                  </div>
                )}
                <form className="space-y-4" onSubmit={handleSubmit}>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">First Name</label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input name="firstName" value={formData.firstName} onChange={handleChange} placeholder="Ayesha" className="pl-10" required />
                  </div>
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Last Name</label>
                  <Input name="lastName" value={formData.lastName} onChange={handleChange} placeholder="Malik" required />
                </div>
              </div>
              
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email Address</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input type="email" name="email" value={formData.email} onChange={handleChange} placeholder="ayesha@clinic.com" className="pl-10" required />
                </div>
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Phone number</label>
                  <div className="relative">
                    <Input type="tel" name="phoneNumber" value={formData.phoneNumber} onChange={handleChange} placeholder="+92 300 1234567" className="pl-3" required />
                  </div>
                </div>
                
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Specialization</label>
                  <div className="relative">
                    <Stethoscope className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input name="specialization" value={formData.specialization} onChange={handleChange} placeholder="e.g. Cardiologist" className="pl-10" required />
                  </div>
                </div>
              </div>
              
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input 
                    type={showPassword ? "text" : "password"} 
                    name="password" 
                    value={formData.password} 
                    onChange={handleChange} 
                    placeholder="••••••••" 
                    className="pl-10 pr-10" 
                    required 
                  />
                  <button 
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 focus:outline-none"
                  >
                    {showPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                  </button>
                </div>
              </div>

              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Confirm Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input 
                    type={showConfirmPassword ? "text" : "password"} 
                    name="confirmPassword" 
                    value={formData.confirmPassword} 
                    onChange={handleChange} 
                    placeholder="••••••••" 
                    className="pl-10 pr-10" 
                    required 
                  />
                  <button 
                    type="button"
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 focus:outline-none"
                  >
                    {showConfirmPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                  </button>
                </div>
              </div>

              <div className="pt-2">
                <h3 className="text-sm font-semibold text-gray-800 dark:text-gray-200 mb-3 border-b border-gray-100 dark:border-gray-700 pb-2">Verification Documents</h3>
                
                <div className="space-y-4">
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Upload Your Degree</label>
                    <div className="relative flex items-center">
                      <Input type="file" onChange={(e) => handleFileChange(e, setDegreeFile)} accept="image/*,.pdf" className="pl-10 py-1.5" required />
                      <Upload className="absolute left-3 h-4 w-4 text-gray-400" />
                    </div>
                    <p className="text-xs text-gray-500">Image or PDF format</p>
                  </div>
                </div>
              </div>

              <Button type="submit" className="w-full mt-6 bg-slate-900 hover:bg-slate-800 dark:bg-slate-700 dark:hover:bg-slate-600" size="lg" disabled={loading}>
                {loading ? "Registering..." : "Register & Continue"}
              </Button>
            </form>
            </>
            )}
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
