"use client";

import { useState } from "react";
import Link from "next/link";
import { motion } from "framer-motion";
import { Stethoscope, Mail, Lock, Eye, EyeOff } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardFooter } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { useAuth } from "@/contexts/AuthContext";
import { Logo } from "@/components/Logo";

export default function LoginPage() {
  const { login } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [formData, setFormData] = useState({
    email: "",
    password: "",
    rememberMe: false
  });
  const [showPassword, setShowPassword] = useState(false);

  const [loginSuccess, setLoginSuccess] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setFormData({ ...formData, [name]: type === "checkbox" ? checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      await login(formData);
      setLoginSuccess(true);
    } catch (err: any) {
      setError(err.message || "Invalid email or password. Please check your credentials.");
    } finally {
      setLoading(false);
    }
  };

  const isUnverifiedEmail = error.toLowerCase().includes("verify your email") || error.toLowerCase().includes("not verified");

  return (
    <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-sky-100/40 via-transparent to-transparent dark:from-sky-900/10">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="w-full max-w-md my-12"
      >
        <div className="text-center mb-8 flex flex-col items-center">
          <Logo href="/" size={44} textSize="text-2xl" />
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Welcome Back</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2">Sign in to your clinical or patient account</p>
        </div>

        <Card className="border-0 shadow-xl shadow-sky-900/5 dark:shadow-none dark:bg-slate-800/80">
          <CardContent className="pt-6">
            {loginSuccess && (
              <div className="mb-5 p-3.5 rounded-lg bg-green-50 dark:bg-green-950/30 text-green-700 dark:text-green-400 text-sm border border-green-200 dark:border-green-800 flex items-center gap-2">
                <span className="font-semibold">Login successful!</span> Redirecting to your dashboard...
              </div>
            )}

            {error && (
              <div className="mb-5 p-3.5 rounded-lg bg-red-50 dark:bg-red-950/30 text-red-600 dark:text-red-400 text-sm border border-red-200 dark:border-red-800 space-y-2">
                <p className="font-medium">{error}</p>
                {isUnverifiedEmail && formData.email && (
                  <Link 
                    href={`/verify-email?email=${encodeURIComponent(formData.email)}`}
                    className="inline-block font-semibold text-[var(--color-primary-600)] dark:text-sky-400 hover:underline text-xs"
                  >
                    Click here to enter your 4-digit verification code &rarr;
                  </Link>
                )}
              </div>
            )}

            <form className="space-y-4" onSubmit={handleSubmit}>
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email Address</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input 
                    type="email" 
                    name="email" 
                    value={formData.email} 
                    onChange={handleChange} 
                    placeholder="doctor@example.com" 
                    className="pl-10 text-base sm:text-sm" 
                    required 
                  />
                </div>
              </div>
              
              <div className="space-y-1">
                <div className="flex justify-between items-center">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Password</label>
                  <Link href="/forgot-password" className="text-xs font-semibold text-[var(--color-primary-600)] dark:text-sky-400 hover:underline">
                    Forgot password?
                  </Link>
                </div>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input 
                    type={showPassword ? "text" : "password"} 
                    name="password" 
                    value={formData.password} 
                    onChange={handleChange} 
                    placeholder="••••••••" 
                    className="pl-10 pr-10 text-base sm:text-sm" 
                    required 
                  />
                  <button 
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 focus:outline-none p-1"
                    aria-label="Toggle password visibility"
                  >
                    {showPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                  </button>
                </div>
              </div>
              
              <div className="flex items-center">
                <input
                  id="remember-me"
                  name="rememberMe"
                  type="checkbox"
                  checked={formData.rememberMe}
                  onChange={handleChange}
                  className="h-4 w-4 rounded border-gray-300 text-[var(--color-primary-600)] focus:ring-[var(--color-primary-500)] cursor-pointer"
                />
                <label htmlFor="remember-me" className="ml-2 block text-sm text-gray-700 dark:text-gray-300 cursor-pointer">
                  Remember me for 30 days
                </label>
              </div>

              <Button type="submit" className="w-full mt-6 bg-[var(--color-primary-600)] hover:bg-blue-700" size="lg" disabled={loading || loginSuccess}>
                {loading ? "Authenticating..." : loginSuccess ? "Redirecting..." : "Sign In"}
              </Button>
            </form>
          </CardContent>

          <CardFooter className="flex flex-col gap-2 border-t border-gray-100 dark:border-slate-700/50 py-4 text-center text-xs text-gray-500 dark:text-gray-400">
            <div>
              New healthcare provider?{" "}
              <Link href="/register/doctor" className="font-semibold text-[var(--color-primary-600)] dark:text-sky-400 hover:underline">
                Join as a Doctor
              </Link>
            </div>
            <div>
              Want an institutional walkthrough?{" "}
              <Link href="/request-demo" className="font-semibold text-[var(--color-primary-600)] dark:text-sky-400 hover:underline">
                Request a Demo
              </Link>
            </div>
          </CardFooter>
        </Card>
      </motion.div>
    </div>
  );
}
