import React from "react";
import Link from "next/link";
import { FolderOpen, Plus, ArrowRight } from "lucide-react";
import { Button } from "@medichp/ui";

export interface EmptyStateProps {
  icon?: React.ReactNode;
  title: string;
  description: string;
  actionText?: string;
  actionHref?: string;
  onAction?: () => void;
  secondaryActionText?: string;
  secondaryActionHref?: string;
  onSecondaryAction?: () => void;
  className?: string;
}

export function EmptyState({
  icon,
  title,
  description,
  actionText,
  actionHref,
  onAction,
  secondaryActionText,
  secondaryActionHref,
  onSecondaryAction,
  className = "",
}: EmptyStateProps) {
  return (
    <div className={`flex flex-col items-center justify-center p-8 sm:p-12 text-center rounded-2xl border-2 border-dashed border-gray-200 dark:border-slate-800 bg-white/50 dark:bg-slate-900/40 backdrop-blur-sm max-w-lg mx-auto ${className}`}>
      <div className="w-16 h-16 rounded-2xl bg-sky-50 dark:bg-sky-950/40 text-[var(--color-primary-600)] dark:text-sky-400 flex items-center justify-center mb-4 ring-1 ring-sky-200 dark:ring-sky-800/60 shadow-sm">
        {icon || <FolderOpen className="w-8 h-8 stroke-[1.75]" />}
      </div>

      <h3 className="text-lg font-bold text-gray-900 dark:text-white mb-2">
        {title}
      </h3>
      
      <p className="text-sm text-gray-500 dark:text-gray-400 mb-6 max-w-sm">
        {description}
      </p>

      {(actionText || secondaryActionText) && (
        <div className="flex flex-col sm:flex-row items-center gap-3 w-full sm:w-auto">
          {actionText && (
            actionHref ? (
              <Link href={actionHref} className="w-full sm:w-auto">
                <Button className="w-full sm:w-auto flex items-center justify-center gap-2 bg-[var(--color-primary-600)] hover:bg-blue-700">
                  <Plus className="w-4 h-4" />
                  <span>{actionText}</span>
                </Button>
              </Link>
            ) : (
              <Button onClick={onAction} className="w-full sm:w-auto flex items-center justify-center gap-2 bg-[var(--color-primary-600)] hover:bg-blue-700">
                <Plus className="w-4 h-4" />
                <span>{actionText}</span>
              </Button>
            )
          )}

          {secondaryActionText && (
            secondaryActionHref ? (
              <Link href={secondaryActionHref} className="w-full sm:w-auto">
                <Button variant="outline" className="w-full sm:w-auto flex items-center justify-center gap-2">
                  <span>{secondaryActionText}</span>
                  <ArrowRight className="w-4 h-4" />
                </Button>
              </Link>
            ) : (
              <Button variant="outline" onClick={onSecondaryAction} className="w-full sm:w-auto flex items-center justify-center gap-2">
                <span>{secondaryActionText}</span>
                <ArrowRight className="w-4 h-4" />
              </Button>
            )
          )}
        </div>
      )}
    </div>
  );
}
