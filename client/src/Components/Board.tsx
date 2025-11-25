import { useState } from "react";

export default function MultiSelectableBoard() {
    const [selected, setSelected] = useState<number[]>([]);

    const PRICE_CONFIG: Record<number, number> = {
        5: 20,
        6: 40,
        7: 80,
        8: 160,
    };

    const MAX_SELECTION = 8;
    const MIN_SELECTION = 5;

    const toggle = (num: number) => {
        setSelected((prev) => {
            if (prev.includes(num)) {
                return prev.filter((n) => n !== num);
            }
            if (prev.length < MAX_SELECTION) {
                return [...prev, num];
            }
            return prev;
        });
    };

    const getPrice = (): string => {
        if (selected.length < MIN_SELECTION) return "—";
        return PRICE_CONFIG[selected.length]?.toString() || "—";
    };

    const isValid = selected.length >= 5 && selected.length <= 8;

    return (
        <div className="w-full max-w-lg mx-auto p-4">
            <div className="grid grid-cols-4 gap-3">
                {Array.from({ length: 16 }, (_, i) => i + 1).map((num) => (
                    <button
                        key={num}
                        onClick={() => toggle(num)}
                        disabled={!selected.includes(num) && selected.length >= 8}
                        className={`aspect-square flex items-center justify-center rounded-xl border text-lg font-semibold transition ${
                            selected.includes(num)
                                ? "!bg-blue-300 !text- !border-blue-600"
                                : selected.length >= MAX_SELECTION
                                    ? "bg-gray-300 border-gray-400 cursor-not-allowed opacity-50"
                                    : "bg-base-200 border-base-300 hover:bg-base-300"
                        }`}
                    >
                        {num}
                    </button>
                ))}
            </div>
            <div className="mt-4 text-center space-y-2">
                <p className="text-lg font-semibold">Valgte numre: {[...selected].sort((a, b) => a - b).join(", ") || "Ingen"}</p>
                <p className={`text-lg font-semibold ${!isValid ? "text-red-500" : ""}`}>
                    Pris: {getPrice()} DKK {!isValid && `(${selected.length}/${MIN_SELECTION}-${MAX_SELECTION})`}
                </p>
            </div>
        </div>
    );
}
