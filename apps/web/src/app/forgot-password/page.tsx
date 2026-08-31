"use client";

import { useState } from "react";
import Link from "next/link";
import { motion } from "framer-motion";
import { Stethoscope, Mail, CheckCircle2 } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardFooter } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { useAuth } from "@/contexts/AuthContext";

export default function ForgotPasswordPage() {
  const { forgotPassword } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [email, setEmail] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email) return;
    
    setLoading(true);
    setError("");
    
    try {
      await forgotPassword({ email });
      setSuccess(true);
    } catch (err: any) {
      setError(err.message || "An unexpected error occurred. Please try again.");
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
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Forgot Password?</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2">
            Enter your email to receive a password reset link.
          </p>
        </div>

        <Card className="border-0 shadow-xl shadow-sky-900/5 dark:shadow-none dark:bg-slate-800/80">
          <CardContent className="pt-6">
            {success ? (
              <div className="text-center py-6">
                <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-green-100 dark:bg-green-900/30 mb-4">
                  <CheckCircle2 className="h-6 w-6 text-green-600 dark:text-green-400" />
                </div>
                <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">Check your email</h3>
                <p className="text-sm text-gray-500 dark:text-gray-400 mb-6">
                  If an account exists for <b>{email}</b>, we have sent a password reset link.
                </p>
                <Link href="/login">
                  <Button variant="outline" className="w-full">
                    Return to sign in
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
                    <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email address</label>
                    <div className="relative">
                      <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                      <Input 
                        type="email" 
                        value={email} 
                        onChange={(e) => setEmail(e.target.value)} 
                        placeholder="john@example.com" 
                        className="pl-10" 
                        required 
                      />
                    </div>
                  </div>

                  <Button type="submit" className="w-full mt-6" size="lg" disabled={loading}>
                    {loading ? "Sending..." : "Send Reset Link"}
                  </Button>
                </form>
              </>
            )}
          </CardContent>
          {!success && (
            <CardFooter className="flex justify-center border-t border-gray-100 dark:border-slate-700/50 pt-6">
              <p className="text-sm text-gray-600 dark:text-gray-400">
                Remember your password?{" "}
                <Link href="/login" className="font-semibold text-[var(--color-primary-600)] hover:underline">
                  Sign in
                </Link>
              </p>
            </CardFooter>
          )}
        </Card>
      </motion.div>
    </div>
  );
}
