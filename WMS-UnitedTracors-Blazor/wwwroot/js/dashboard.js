window.renderDashboardCharts = function (data) {
    if (typeof Chart === 'undefined') return;

    const donutOpts = {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '75%',
        plugins: { legend: { display: false }, tooltip: { enabled: true } }
    };

    // --- Transaction Status Donut ---
    const ctxDonut = document.getElementById('donutChartStatus');
    if (ctxDonut) {
        if (window.chartStatus) window.chartStatus.destroy();
        window.chartStatus = new Chart(ctxDonut, {
            type: 'doughnut',
            data: {
                labels: ['Approved', 'Rejected', 'Pending'],
                datasets: [{
                    data: [data.donutData.approved, data.donutData.rejected, data.donutData.pending],
                    backgroundColor: ['#1a7a30', '#d94040', '#e8a000'],
                    borderWidth: 0, hoverOffset: 6
                }]
            },
            options: donutOpts
        });
    }

    // --- Merch Item Status Donut ---
    const ctxMerch = document.getElementById('donutChartMerch');
    if (ctxMerch) {
        if (window.chartMerch) window.chartMerch.destroy();
        window.chartMerch = new Chart(ctxMerch, {
            type: 'doughnut',
            data: {
                labels: ['Available', 'Low Stock', 'Out of Stock'],
                datasets: [{
                    data: [data.merchStatus.available, data.merchStatus.low_stock, data.merchStatus.out_of_stock],
                    backgroundColor: ['#1a7a30', '#e8a000', '#d94040'],
                    borderWidth: 0, hoverOffset: 6
                }]
            },
            options: donutOpts
        });
    }

    // --- Borrow Item Status Donut ---
    const ctxBorrow = document.getElementById('donutChartBorrow');
    if (ctxBorrow) {
        if (window.chartBorrow) window.chartBorrow.destroy();
        window.chartBorrow = new Chart(ctxBorrow, {
            type: 'doughnut',
            data: {
                labels: ['Overdue', 'Available', 'Borrowed'],
                datasets: [{
                    data: [data.borrowStatus.overdue, data.borrowStatus.available, data.borrowStatus.borrowed],
                    backgroundColor: ['#d94040', '#1a7a30', '#3b82f6'],
                    borderWidth: 0, hoverOffset: 6
                }]
            },
            options: donutOpts
        });
    }
};

window.renderReportCharts = function (data) {
    if (typeof Chart === 'undefined') return;

    // Clear old charts if they exist
    if (window.chartP) window.chartP.destroy();
    if (window.chartB) window.chartB.destroy();
    if (window.chartES) window.chartES.destroy();

    const chartPeminjaman = document.getElementById('chartPeminjaman');
    const chartBarangKeluar = document.getElementById('chartBarangKeluar');
    const chartES = document.getElementById('chartExecutiveSummary');

    if (chartPeminjaman) {
        window.chartP = new Chart(chartPeminjaman, {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Peminjaman',
                    data: data.peminjaman,
                    backgroundColor: '#e8a000',
                    borderRadius: 4,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    y: { beginAtZero: true, ticks: { precision: 0 } }
                }
            }
        });
    }

    if (chartBarangKeluar) {
        window.chartB = new Chart(chartBarangKeluar, {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Barang Keluar',
                    data: data.barangKeluar,
                    backgroundColor: '#1a1a1a',
                    borderRadius: 4,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    y: { beginAtZero: true, ticks: { precision: 0 } }
                }
            }
        });
    }

    if (chartES) {
        window.chartES = new Chart(chartES, {
            type: 'doughnut',
            data: {
                labels: ['Dipinjam', 'Sudah Kembali', 'Masih Dipinjam'],
                datasets: [{
                    data: [data.totalBorrowed, data.totalReturned, data.masihDipinjam],
                    backgroundColor: ['#e8a000', '#1a7a30', '#d94040'],
                    borderWidth: 0,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '65%',
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 16,
                            usePointStyle: true,
                            font: { size: 11, weight: '600' },
                            color: '#3a3835'
                        }
                    }
                }
            }
        });
    }
};

window.switchItemStatus = function(tab) {
    const btnMerch = document.getElementById('btnMerch');
    const btnBorrow = document.getElementById('btnBorrow');
    const viewMerch = document.getElementById('itemStatusMerch');
    const viewBorrow = document.getElementById('itemStatusBorrow');

    if (tab === 'merch') {
        btnMerch.classList.add('bg-white', 'shadow-sm', 'text-[#1a1a1a]');
        btnMerch.classList.remove('text-[#8a8880]', 'bg-transparent');
        btnBorrow.classList.add('text-[#8a8880]', 'bg-transparent');
        btnBorrow.classList.remove('bg-white', 'shadow-sm', 'text-[#1a1a1a]');
        
        viewMerch.classList.remove('hidden');
        viewMerch.classList.add('flex');
        viewBorrow.classList.add('hidden');
        viewBorrow.classList.remove('flex');
    } else {
        btnBorrow.classList.add('bg-white', 'shadow-sm', 'text-[#1a1a1a]');
        btnBorrow.classList.remove('text-[#8a8880]', 'bg-transparent');
        btnMerch.classList.add('text-[#8a8880]', 'bg-transparent');
        btnMerch.classList.remove('bg-white', 'shadow-sm', 'text-[#1a1a1a]');
        
        viewBorrow.classList.remove('hidden');
        viewBorrow.classList.add('flex');
        viewMerch.classList.add('hidden');
        viewMerch.classList.remove('flex');
    }
};

window.checkLowStockScroll = function(el) {
    const indicator = document.getElementById('scrollIndicator');
    if (!indicator) return;
    
    if (el.scrollHeight - el.scrollTop <= el.clientHeight + 10) {
        indicator.style.opacity = '0';
    } else {
        indicator.style.opacity = '1';
    }
};