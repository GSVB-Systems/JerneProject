export default function ThinBoard({
    selectedNumbers,
    creationTimestamp,
    weeksRemaining,
    hasWon
}: {
    selectedNumbers: number[];
    creationTimestamp?: number;
    weeksRemaining?: number;
    hasWon?: boolean;
}) {
    const getWeekAndYear = (unix?: number) => {
        if (!unix) return "";
        const date = new Date(unix * 1000);
        const yearStart = new Date(date.getFullYear(), 0, 1);
        const weekNumber = Math.ceil((date.getTime() - yearStart.getTime()) / 86400000 / 7);
        const year = date.getFullYear();
        return `Uge ${weekNumber}, ${year}`;
    };

    const normalizedWeeksRemaining = typeof weeksRemaining === "number"
        ? Math.max(0, Math.floor(weeksRemaining))
        : undefined;
    const boardWrapperClasses = `border-2 rounded-lg h-16 sm:h-20 flex items-center justify-center overflow-x-auto flex-1 ${
        hasWon ? "border-green-500 bg-green-50 shadow-lg" : "border-gray-400 bg-gray-50"
    }`;

    return (
        <div className="w-full flex items-center justify-center p-4">
            <div className="w-full max-w-2xl">
                {creationTimestamp && (
                    <div className="text-center text-sm font-semibold mb-2 text-gray-700">
                        {getWeekAndYear(creationTimestamp)}
                    </div>
                )}

                <div className={`${boardWrapperClasses} relative`}>
                    {hasWon && (
                        <div className="absolute -mt-12 self-start text-xs font-semibold text-green-700" aria-live="polite">
                            <span className="px-2 py-1 bg-green-100 border border-green-500 rounded-full">Winner</span>
                            <span className="sr-only">This board has already won</span>
                        </div>
                    )}
                    <div className="flex gap-2">
                        <div className="flex items-center justify-center gap-4">
                            {typeof normalizedWeeksRemaining === "number" && (
                                <div className="text-3xl sm:text-5xl font-bold text-gray-800 text-center">
                                    {normalizedWeeksRemaining}
                                </div>
                            )}
                            {[...selectedNumbers].sort((a, b) => a - b).map((num) => (
                                <div
                                    key={num}
                                    className="w-8 h-8 sm:w-12 sm:h-12 flex-shrink-0 flex items-center justify-center rounded-lg border-2 border-red-600 bg-red-300 text-sm sm:text-lg font-bold"
                                >
                                    {num}
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}