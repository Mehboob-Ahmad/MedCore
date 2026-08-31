"use client";

import { useState, useEffect, Suspense } from "react";
import Link from "next/link";
import { useSearchParams, useRouter } from "next/navigation";
import { motion } from "framer-motion";
import { Stethoscope, Lock, CheckCircle2 } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { useAuth } from "@/contexts/AuthContext";

function ResetPasswordForm() {
  const { resetPassword } = useAuth();
  const searchParams = useSearchParams();
  const router = useRouter();
  
  const token = searchParams.get("token");
  const email = searchParams.get("email");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  useEffect(() => {
    if (!token || !email) {
      setError("Invalid or missing reset token.");
    }
  }, [token, email]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token || !email) return;
    
    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }
    
    // Basic frontend strength check
    if (password.length < 8) {
      setError("Password must be at least 8 characters long.");
      return;
    }

    setLoading(true);
    setError("");
    
    try {
      await resetPassword({ 
        email, 
        token, 
        newPassword: password 
      });
      setSuccess(true);
    } catch (err: any) {
      setError(err.message || "An error occurred while resetting your password. The token may be expired or invalid.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card className="border-0 shadow-xl shadow-sky-900/5 dark:shadow-none dark:bg-slate-800/80">
      <CardContent className="pt-6">
        {success ? (
          <div className="text-center py-6">
            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-green-100 dark:bg-green-900/30 mb-4">
              <CheckCircle2 className="h-6 w-6 text-green-600 dark:text-green-400" />
            </div>
            <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">Password Reset Successful</h3>
            <p className="text-sm text-gray-500 dark:text-gray-400 mb-6">
              Your password has been successfully updated. You can now sign in with your new password.
            </p>
            <Link href="/login">
              <Button className="w-full">
                Sign In
              </Button>
            </Link>
          </div>
        ) : (
          <>
            {error && (
              <div className="mb-4 p-3 rounded bg-red-50 text-red-600 text-sm border border-red-200">
                {error}
              </div>
            )}
            <form className="space-y-4" onSubmit={handleSubmit}>
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">New Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input 
                    type="password" 
                    value={password} 
                    onChange={(e) => setPassword(e.target.value)} 
                    placeholder="••••••••" 
                    className="pl-10" 
                    required 
                    disabled={!token || !email}
                  />
                </div>
              </div>
              
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Confirm New Password</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input 
                    type="password" 
                    value={confirmPassword} 
                    onChange={(e) => setConfirmPassword(e.target.value)} 
                    placeholder="••••••••" 
                    className="pl-10" 
                    required 
                    disabled={!token || !email}
                  />
                </div>
              </div>

              <Button type="submit" className="w-full mt-6" size="lg" disabled={loading || !token || !email}>
                {loading ? "Resetting..." : "Reset Password"}
              </Button>
            </form>
          </>
        )}
      </CardContent>
    </Card>
  );
}

export default function ResetPasswordPage() {
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
          <p className="text-gray-500 dark:text-gray-400 mt-2">
            Enter your new password below.
          </p>
        </div>

        <Suspense fallback={<div className="text-center py-6">Loading...</div>}>
          <ResetPasswordForm />
        </Suspense>
      </motion.div>
    </div>
  );
}
