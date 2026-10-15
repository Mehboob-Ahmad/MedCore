"use client";

import { useState, useEffect, useRef, Suspense } from "react";
import { useSearchParams } from "next/navigation";
import Link from "next/link";
import { motion } from "framer-motion";
import { Stethoscope, Mail, ShieldCheck, RefreshCw } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent } from "@medichp/ui";
import { AuthService } from "@medichp/api-client";
import { Logo } from "@/components/Logo";

function VerifyEmailContent() {
  const searchParams = useSearchParams();
  const email = searchParams.get("email") || "";
  
  const [otp, setOtp] = useState(["", "", "", ""]);
  const [loading, setLoading] = useState(false);
  const [resending, setResending] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [resendCooldown, setResendCooldown] = useState(0);

  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);

  // Cooldown timer
  useEffect(() => {
    if (resendCooldown > 0) {
      const timer = setTimeout(() => setResendCooldown(resendCooldown - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendCooldown]);

  const handleOtpChange = (index: number, value: string) => {
    if (!/^\d*$/.test(value)) return; // Only digits
    
    const newOtp = [...otp];
    newOtp[index] = value.slice(-1); // Take last digit only
    setOtp(newOtp);

    // Auto-advance to next input
    if (value && index < 3) {
      inputRefs.current[index + 1]?.focus();
    }
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Backspace" && !otp[index] && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
  };

  const handlePaste = (e: React.ClipboardEvent) => {
    e.preventDefault();
    const pasted = e.clipboardData.getData("text").replace(/\D/g, "").slice(0, 4);
    const newOtp = [...otp];
    for (let i = 0; i < pasted.length; i++) {
      newOtp[i] = pasted[i];
    }
    setOtp(newOtp);
    // Focus last filled input or the one after
    const focusIndex = Math.min(pasted.length, 3);
    inputRefs.current[focusIndex]?.focus();
  };

  const [customEmail, setCustomEmail] = useState("");
  const activeEmail = email || customEmail;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const otpCode = otp.join("");
    if (otpCode.length !== 4) {
      setError("Please enter the full 4-digit code.");
      return;
    }

    setLoading(true);
    setError("");

    try {
      const res = await AuthService.verifyEmail({ email: activeEmail, otpCode });
      if (res.success) {
        setSuccess(true);
      } else {
        setError(res.message || "Verification failed. Please check the code and try again.");
      }
    } catch (err: any) {
      setError(err.message || "Verification failed. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const handleResend = async () => {
    if (resendCooldown > 0 || !activeEmail) return;
    setResending(true);
    setError("");

    try {
      await AuthService.resendVerification({ email: activeEmail });
      setResendCooldown(60); // 60 second cooldown
    } catch (err: any) {
      setError(err.message || "Could not resend code. Please try again.");
    } finally {
      setResending(false);
    }
  };

  if (!email && !customEmail) {
    return (
      <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900">
        <Card className="w-full max-w-md border-0 shadow-xl dark:bg-slate-800/80">
          <CardContent className="pt-6 space-y-4">
            <div className="w-12 h-12 bg-sky-50 dark:bg-sky-950/40 text-[var(--color-primary-600)] rounded-xl flex items-center justify-center mx-auto">
              <Mail className="w-6 h-6" />
            </div>
            <div className="text-center">
              <h2 className="text-xl font-bold text-gray-900 dark:text-white">Verify Your Email</h2>
              <p className="text-gray-500 dark:text-gray-400 text-sm mt-1">
                Enter your account email to enter your 4-digit verification code.
              </p>
            </div>

            <form onSubmit={(e) => {
              e.preventDefault();
              const input = (e.currentTarget.elements.namedItem("inputEmail") as HTMLInputElement)?.value;
              if (input) setCustomEmail(input);
            }} className="space-y-4 pt-2">
              <input
                name="inputEmail"
                type="email"
                required
                placeholder="doctor@example.com"
                className="w-full px-4 py-2.5 rounded-lg border border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-700 text-sm focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-600)]"
              />
              <Button type="submit" className="w-full bg-[var(--color-primary-600)] hover:bg-blue-700" size="lg">
                Continue to Code Entry &rarr;
              </Button>
            </form>

            <div className="pt-2 text-center">
              <Link href="/login" className="text-xs text-[var(--color-primary-600)] dark:text-sky-400 hover:underline">
                Return to Login
              </Link>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

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
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Verify Your Email</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2 text-sm">
            We sent a 4-digit code to <span className="font-semibold text-gray-800 dark:text-gray-200">{activeEmail}</span>
          </p>
        </div>

        <Card className="border-0 shadow-xl shadow-sky-900/5 dark:shadow-none dark:bg-slate-800/80">
          <CardContent className="pt-6">
            {success ? (
              <div className="text-center space-y-6 py-8">
                <motion.div 
                  initial={{ scale: 0 }}
                  animate={{ scale: 1 }}
                  transition={{ type: "spring", stiffness: 200, damping: 15 }}
                  className="w-16 h-16 bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400 rounded-full flex items-center justify-center mx-auto"
                >
                  <ShieldCheck className="w-8 h-8" />
                </motion.div>
                <div>
                  <h2 className="text-xl font-bold text-gray-900 dark:text-white">Email Verified!</h2>
                  <p className="text-gray-500 dark:text-gray-400 mt-2">Your account is now active. You can sign in.</p>
                </div>
                <Link href="/login" className="inline-block w-full">
                  <Button className="w-full" size="lg">
                    Go to Login
                  </Button>
                </Link>
              </div>
            ) : (
              <>
                {error && (
                  <div className="mb-4 p-3 rounded bg-red-50 text-red-600 text-sm border border-red-200 dark:bg-red-900/20 dark:text-red-400 dark:border-red-800">
                    {error}
                  </div>
                )}
                <form className="space-y-6" onSubmit={handleSubmit}>
                  {/* OTP Input */}
                  <div className="flex justify-center gap-3" onPaste={handlePaste}>
                    {otp.map((digit, index) => (
                      <input
                        key={index}
                        ref={(el) => { inputRefs.current[index] = el; }}
                        type="text"
                        inputMode="numeric"
                        maxLength={1}
                        value={digit}
                        onChange={(e) => handleOtpChange(index, e.target.value)}
                        onKeyDown={(e) => handleKeyDown(index, e)}
                        className="w-14 h-14 text-center text-2xl font-bold rounded-lg border-2 border-gray-200 dark:border-slate-600 bg-white dark:bg-slate-700 text-gray-900 dark:text-white focus:border-[var(--color-primary-500)] focus:ring-2 focus:ring-[var(--color-primary-500)]/20 outline-none transition-all"
                        autoFocus={index === 0}
                      />
                    ))}
                  </div>

                  <Button type="submit" className="w-full" size="lg" disabled={loading || otp.join("").length !== 4}>
                    {loading ? "Verifying..." : "Verify Email"}
                  </Button>
                </form>

                <div className="mt-6 text-center">
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    Didn&apos;t receive the code?
                  </p>
                  <button
                    onClick={handleResend}
                    disabled={resendCooldown > 0 || resending}
                    className="mt-2 inline-flex items-center gap-2 text-sm font-medium text-[var(--color-primary-600)] hover:underline disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    <RefreshCw className={`w-4 h-4 ${resending ? 'animate-spin' : ''}`} />
                    {resendCooldown > 0 ? `Resend in ${resendCooldown}s` : resending ? "Sending..." : "Resend Code"}
                  </button>
                </div>
              </>
            )}
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}

export default function VerifyEmailPage() {
  return (
    <Suspense fallback={
      <div className="flex-1 min-h-[60vh] flex items-center justify-center">
        <div className="w-8 h-8 border-4 border-sky-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    }>
      <VerifyEmailContent />
    </Suspense>
  );
}
