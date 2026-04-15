"use client";

import DatePicker from "react-datepicker";
import { format, isValid, parseISO } from "date-fns";
import { ptBR } from "date-fns/locale";
import { cn } from "@/lib/utils";
import "react-datepicker/dist/react-datepicker.css";

type DatePickerInputProps = {
  value: string;
  onChange: (value: string) => void;
  className?: string;
  placeholder?: string;
  disabled?: boolean;
  min?: string;
  max?: string;
};

const toDate = (value: string): Date | null => {
  if (!value) {
    return null;
  }

  const parsed = parseISO(value);
  return isValid(parsed) ? parsed : null;
};

export function DatePickerInput({
  value,
  onChange,
  className,
  placeholder = "Selecione a data",
  disabled,
  min,
  max,
}: DatePickerInputProps) {
  return (
    <DatePicker
      selected={toDate(value)}
      onChange={(date: Date | null) =>
        onChange(date ? format(date, "yyyy-MM-dd") : "")
      }
      dateFormat="dd/MM/yyyy"
      locale={ptBR}
      disabled={disabled}
      minDate={toDate(min ?? "") ?? undefined}
      maxDate={toDate(max ?? "") ?? undefined}
      placeholderText={placeholder}
      showPopperArrow={false}
      isClearable
      className={cn("h-10 w-full rounded-md border bg-background px-3 text-sm", className)}
    />
  );
}
