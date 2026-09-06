"use client";

import { useState, useEffect, useRef } from "react";
import { AiService } from "@medichp/api-client";
import { Sparkles, Send, Loader2, User, Bot } from "lucide-react";

export default function PatientAIPage() {
  const [messages, setMessages] = useState<any[]>([]);
  const [input, setInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isInitializing, setIsInitializing] = useState(true);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    fetchHistory();
  }, []);

  useEffect(() => {
    scrollToBottom();
  }, [messages, isLoading]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  const fetchHistory = async () => {
    try {
      const response = await AiService.getHistory();
      if (response.success && response.data) {
        setMessages(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch chat history:", error);
    } finally {
      setIsInitializing(false);
    }
  };

  const handleSend = async (e?: React.FormEvent, suggestionText?: string) => {
    e?.preventDefault();
    const textToSend = suggestionText || input;
    if (!textToSend.trim() || isLoading) return;

    const userMessage = { role: "user", content: textToSend };
    setMessages((prev) => [...prev, userMessage]);
    setInput("");
    setIsLoading(true);

    try {
      const response = await AiService.ask(textToSend);
      if (response.success) {
        setMessages((prev) => [
          ...prev,
          { role: "model", content: response.data.answer }
        ]);
      }
    } catch (error: any) {
      console.error("Failed to get AI response:", error);
      setMessages((prev) => [
        ...prev,
        { role: "model", content: "I'm sorry, I'm having trouble connecting right now. Please try again." }
      ]);
    } finally {
      setIsLoading(false);
    }
  };

  const suggestions = [
    "What are the clinic's operating hours?",
    "How do I book an appointment?",
    "Can you explain my prescription?",
    "Where is the clinic located?"
  ];

  return (
    <div className="flex flex-col h-[calc(100vh-120px)] bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-gray-100 dark:border-slate-800 overflow-hidden">
      {/* Header */}
      <div className="p-4 border-b border-gray-100 dark:border-slate-800 bg-sky-50 dark:bg-slate-800 flex items-center space-x-3">
        <div className="bg-[var(--color-primary-600)] p-2 rounded-xl">
          <Sparkles className="w-5 h-5 text-white" />
        </div>
        <div>
          <h1 className="font-semibold text-gray-900 dark:text-white">AI Assistant</h1>
          <p className="text-xs text-gray-500 dark:text-gray-400">Ask medical or platform-related questions</p>
        </div>
      </div>

      {/* Chat Area */}
      <div className="flex-1 overflow-y-auto p-4 space-y-4">
        {isInitializing ? (
          <div className="flex justify-center items-center h-full">
            <Loader2 className="w-8 h-8 animate-spin text-[var(--color-primary-600)]" />
          </div>
        ) : messages.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-full text-center space-y-6">
            <div className="bg-sky-50 dark:bg-slate-800 p-4 rounded-full">
              <Bot className="w-12 h-12 text-[var(--color-primary-600)]" />
            </div>
            <div>
              <h2 className="text-lg font-medium text-gray-900 dark:text-white">How can I help you today?</h2>
              <p className="text-sm text-gray-500 dark:text-gray-400 mt-2 max-w-sm">
                I can help you understand your medical reports, check clinic hours, or guide you through booking an appointment.
              </p>
            </div>
            
            <div className="flex flex-wrap justify-center gap-2 max-w-md">
              {suggestions.map((suggestion, idx) => (
                <button
                  key={idx}
                  onClick={() => handleSend(undefined, suggestion)}
                  className="text-xs px-3 py-2 bg-gray-100 dark:bg-slate-800 hover:bg-gray-200 dark:hover:bg-slate-700 text-gray-700 dark:text-gray-300 rounded-full transition-colors"
                >
                  {suggestion}
                </button>
              ))}
            </div>
          </div>
        ) : (
          messages.map((msg, idx) => (
            <div key={idx} className={`flex ${msg.role === 'user' ? 'justify-end' : 'justify-start'}`}>
              <div className={`max-w-[80%] rounded-2xl p-4 flex gap-3 ${
                msg.role === 'user' 
                  ? 'bg-[var(--color-primary-600)] text-white' 
                  : 'bg-gray-100 dark:bg-slate-800 text-gray-800 dark:text-gray-200'
              }`}>
                {msg.role === 'user' ? null : (
                  <div className="mt-1">
                    <Bot className="w-5 h-5 opacity-70" />
                  </div>
                )}
                <div className="whitespace-pre-wrap text-sm leading-relaxed">
                  {msg.content}
                </div>
              </div>
            </div>
          ))
        )}
        
        {isLoading && (
          <div className="flex justify-start">
            <div className="max-w-[80%] rounded-2xl p-4 bg-gray-100 dark:bg-slate-800 flex gap-3 items-center">
              <Bot className="w-5 h-5 text-gray-500" />
              <Loader2 className="w-4 h-4 animate-spin text-gray-500" />
            </div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      {/* Input Area */}
      <div className="p-4 bg-white dark:bg-slate-900 border-t border-gray-100 dark:border-slate-800">
        <form onSubmit={handleSend} className="relative flex items-center">
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            placeholder="Type your question..."
            disabled={isLoading}
            className="w-full bg-gray-100 dark:bg-slate-800 text-gray-900 dark:text-white border-0 rounded-full pl-5 pr-12 py-3 focus:ring-2 focus:ring-[var(--color-primary-500)] outline-none"
          />
          <button
            type="submit"
            disabled={!input.trim() || isLoading}
            className="absolute right-2 p-2 bg-[var(--color-primary-600)] text-white rounded-full hover:bg-[var(--color-primary-700)] disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            <Send className="w-4 h-4" />
          </button>
        </form>
      </div>
    </div>
  );
}
