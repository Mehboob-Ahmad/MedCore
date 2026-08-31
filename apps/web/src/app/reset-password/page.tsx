"use client";
import React from "react";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useSearchParams, useRouter } from "next/navigation";
import { motion } from "framer-motion";
import { Stethoscope, Lock, ArrowLeft, CheckCircle } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardFooter } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { AuthService } from "@medichp/api-client";

function ResetPasswordContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  
  const [token, setToken] = useState("");
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  
  const [formData, setFormData] = useState({
    newPassword: "",
    confirmPassword: ""
  });

  useEffect(() => {
    const tokenParam = searchParams.get("token");
    const emailParam = searchParams.get("email");
    if (tokenParam) setToken(tokenParam);
    if (emailParam) setEmail(emailParam);
  }, [searchParams]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    
    if (formData.newPassword !== formData.confirmPassword) {
      setError("Passwords do not match.");
      setLoading(false);
      return;
    }
    
    if (!token || !email) {
      setError("Invalid reset link. Missing token or email.");
      setLoading(false);
      return;
    }
    
    try {
      await AuthService.resetPassword({
        email,
        token,
        newPassword: formData.newPassword
      });
      setSuccess(true);
    } catch (err: any) {
      setError(err.message || "Failed to reset password. The link might be expired or invalid.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-sky-100/40 via-transparent to-transparent dark:from-sky-900/10">
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
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Create New Password</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2">Please enter your new password below.</p>
        </div>

        <Card className="border-0 shadow-xl shadow-sky-900/5 dark:shadow-none dark:bg-slate-800/80">
          <CardContent className="pt-6">
            {error && (
              <div className="mb-4 p-3 rounded bg-red-50 text-red-600 text-sm border border-red-200">
                {error}
              </div>
            )}
            
            {success ? (
              <div className="text-center space-y-4 py-4">
                <div className="flex justify-center">
                  <CheckCircle className="w-16 h-16 text-green-500" />
                </div>
                <h3 className="text-xl font-medium text-gray-900 dark:text-white">Password Reset Complete</h3>
                <p className="text-sm text-gray-500 dark:text-gray-400 mb-6">
                  Your password has been successfully reset. You can now log in with your new password.
                </p>
                <Button  className="w-full" size="lg">
                  <Link href="/login">
                    Go to Log In
                  </Link>
                </Button>
              </div>
            ) : (
              <form className="space-y-4" onSubmit={handleSubmit}>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">New Password</label>
                  <div className="relative">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input 
                      type="password" 
                      name="newPassword"
                      value={formData.newPassword} 
                      onChange={handleChange} 
                      placeholder="••••••••" 
                      className="pl-10" 
                      required 
                      minLength={8}
                    />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Confirm New Password</label>
                  <div className="relative">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input 
                      type="password" 
                      name="confirmPassword"
                      value={formData.confirmPassword} 
                      onChange={handleChange} 
                      placeholder="••••••••" 
                      className="pl-10" 
                      required 
                      minLength={8}
                    />
                  </div>
                </div>

                <Button type="submit" className="w-full mt-6" size="lg" disabled={loading}>
                  {loading ? "Resetting Password..." : "Reset Password"}
                </Button>
              </form>
            )}
          </CardContent>
          {!success && (
            <CardFooter className="flex justify-center border-t border-gray-100 dark:border-slate-700/50 pt-6">
              <Link href="/login" className="flex items-center text-sm font-semibold text-gray-600 dark:text-gray-400 hover:text-[var(--color-primary-600)] transition-colors">
                <ArrowLeft className="w-4 h-4 mr-2" />
                Back to Sign In
              </Link>
            </CardFooter>
          )}
        </Card>
      </motion.div>
    </div>
  );
}

export default function ResetPasswordPage() {
  return (
    <React.Suspense fallback={<div>Loading...</div>}>
      <ResetPasswordContent />
    </React.Suspense>
  );
}
