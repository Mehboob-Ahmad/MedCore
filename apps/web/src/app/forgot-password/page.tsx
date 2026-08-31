"use client";

import { useState } from "react";
import Link from "next/link";
import { motion } from "framer-motion";
import { Stethoscope, Mail, ArrowLeft } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardFooter } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { AuthService } from "@medichp/api-client";

export default function ForgotPasswordPage() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [email, setEmail] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    setSuccess(false);
    
    try {
      await AuthService.forgotPassword({ email });
      setSuccess(true);
    } catch (err: any) {
      // For security, we might want to show success even if the email doesn't exist,
      // but if the API returns a generic success for all valid emails, we just use it.
      setError(err.message || "Failed to process request. Please try again.");
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
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Forgot Password</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2">Enter your email to receive a password reset link.</p>
        </div>

        <Card className="border-0 shadow-xl shadow-sky-900/5 dark:shadow-none dark:bg-slate-800/80">
          <CardContent className="pt-6">
            {error && (
              <div className="mb-4 p-3 rounded bg-red-50 text-red-600 text-sm border border-red-200">
                {error}
              </div>
            )}
            
            {success ? (
              <div className="text-center space-y-4">
                <div className="p-4 rounded-lg bg-green-50 text-green-700 border border-green-200 mb-4">
                  If an account exists for <strong>{email}</strong>, you will receive a password reset link shortly.
                </div>
                <Button  variant="outline" className="w-full">
                  <Link href="/login">
                    <ArrowLeft className="w-4 h-4 mr-2" />
                    Back to Sign In
                  </Link>
                </Button>
              </div>
            ) : (
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
                  {loading ? "Sending link..." : "Send Reset Link"}
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
