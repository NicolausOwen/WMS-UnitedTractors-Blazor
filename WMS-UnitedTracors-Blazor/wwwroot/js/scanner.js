window.initProductScanner = function() {
    if (typeof ZXing === 'undefined') {
        console.error("ZXing library not loaded");
        return;
    }
    const codeReader = new ZXing.BrowserMultiFormatReader();
    const startButton = document.getElementById('startButton');
    const resetButton = document.getElementById('resetButton');
    const scannerContainer = document.getElementById('scannerContainer');
    const skuInput = document.getElementById('skuInput');

    if (!startButton || !resetButton || !scannerContainer || !skuInput) return;

    // Beep sound
    const audioCtx = new (window.AudioContext || window.webkitAudioContext)();
    function playBeep() {
        if (audioCtx.state === 'suspended') audioCtx.resume();
        const o = audioCtx.createOscillator();
        const g = audioCtx.createGain();
        o.connect(g); g.connect(audioCtx.destination);
        o.type = 'sine'; o.frequency.setValueAtTime(800, audioCtx.currentTime);
        g.gain.setValueAtTime(0.1, audioCtx.currentTime);
        o.start(); o.stop(audioCtx.currentTime + 0.15);
    }

    const newStartBtn = startButton.cloneNode(true);
    startButton.parentNode.replaceChild(newStartBtn, startButton);
    const newResetBtn = resetButton.cloneNode(true);
    resetButton.parentNode.replaceChild(newResetBtn, resetButton);

    newStartBtn.addEventListener('click', async () => {
        newStartBtn.classList.add('hidden');
        newResetBtn.classList.remove('hidden');
        scannerContainer.classList.remove('hidden');
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ video: true });
            stream.getTracks().forEach(track => track.stop());
            const devices = await codeReader.listVideoInputDevices();
            let selectedDeviceId = undefined;
            if (devices && devices.length > 0) {
                selectedDeviceId = devices[0].deviceId;
                const backCamera = devices.find(d => /back|rear|environment/i.test(d.label));
                if (backCamera) selectedDeviceId = backCamera.deviceId;
            }
            codeReader.decodeFromVideoDevice(selectedDeviceId, 'video', (result, err) => {
                if (result) {
                    playBeep();
                    skuInput.value = result.getText();
                    skuInput.dispatchEvent(new Event('change', { bubbles: true }));
                    newResetBtn.click();
                }
            });
        } catch (error) {
            console.error("Camera access error:", error);
            alert("Could not access camera. Please ensure permissions are granted.");
            newResetBtn.click();
        }
    });

    newResetBtn.addEventListener('click', () => {
        codeReader.reset();
        newResetBtn.classList.add('hidden');
        newStartBtn.classList.remove('hidden');
        scannerContainer.classList.add('hidden');
    });
}

window.initScannerIndex = function(dotNetHelper) {
    if (typeof ZXing === 'undefined') {
        console.error("ZXing library not loaded");
        return;
    }
    const codeReader = new ZXing.BrowserMultiFormatReader();
    const startButton   = document.getElementById('startButton');
    const resetButton   = document.getElementById('resetButton');
    const scannedBadge  = document.getElementById('scannedBadge');
    const cameraIdle    = document.getElementById('cameraIdle');
    const scanOverlay   = document.getElementById('scanOverlay');

    if (!startButton || !resetButton) return;

    // Beep sound
    const audioCtx = new (window.AudioContext || window.webkitAudioContext)();
    function playBeep() {
        if (audioCtx.state === 'suspended') audioCtx.resume();
        const o = audioCtx.createOscillator();
        const g = audioCtx.createGain();
        o.connect(g); 
        g.connect(audioCtx.destination);
        o.type = 'sine';
        o.frequency.setValueAtTime(880, audioCtx.currentTime);
        g.gain.setValueAtTime(0.12, audioCtx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, audioCtx.currentTime + 0.2);
        o.start();
        o.stop(audioCtx.currentTime + 0.2);
    }

    const newStartBtn = startButton.cloneNode(true);
    startButton.parentNode.replaceChild(newStartBtn, startButton);
    const newResetBtn = resetButton.cloneNode(true);
    resetButton.parentNode.replaceChild(newResetBtn, resetButton);

    newStartBtn.addEventListener('click', async () => {
        newStartBtn.style.display = 'none';
        newResetBtn.style.display = 'block';
        cameraIdle.style.display = 'none';
        scanOverlay.style.display = 'flex';
        scannedBadge.style.display = 'none';
        
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ video: true });
            stream.getTracks().forEach(track => track.stop());
            const devices = await codeReader.listVideoInputDevices();
            let selectedDeviceId = undefined;
            
            if (devices && devices.length > 0) {
                selectedDeviceId = devices[0].deviceId;
                const backCamera = devices.find(d => /back|rear|environment/i.test(d.label));
                if (backCamera) selectedDeviceId = backCamera.deviceId;
            }
            
            codeReader.decodeFromVideoDevice(selectedDeviceId, 'video', (result, err) => {
                if (result) {
                    playBeep();
                    const sku = result.getText();
                    dotNetHelper.invokeMethodAsync('SetScannedSku', sku);
                    
                    scannedBadge.style.display = 'inline-flex';
                    codeReader.reset();
                    newResetBtn.style.display = 'none';
                    newStartBtn.style.display = 'block';
                    newStartBtn.querySelector('span').innerHTML = `
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 10l4.553-2.069A1 1 0 0121 8.882V15.118a1 1 0 01-1.447.91L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z"/></svg>
                        Scan Lagi
                    `;
                    cameraIdle.style.display = 'flex';
                    scanOverlay.style.display = 'none';
                }
                if (err && !(err instanceof ZXing.NotFoundException)) {
                    console.error(err);
                }
            });
        } catch (error) {
            console.error("Camera access error:", error);
            alert("Tidak dapat mengakses kamera. Pastikan izin kamera sudah diberikan.");
            newResetBtn.style.display = 'none';
            newStartBtn.style.display = 'block';
            cameraIdle.style.display = 'flex';
            scanOverlay.style.display = 'none';
        }
    });

    newResetBtn.addEventListener('click', () => {
        codeReader.reset();
        newResetBtn.style.display = 'none';
        newStartBtn.style.display = 'block';
        newStartBtn.querySelector('span').innerHTML = `
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 10l4.553-2.069A1 1 0 0121 8.882V15.118a1 1 0 01-1.447.91L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z"/></svg>
            Mulai Kamera
        `;
        cameraIdle.style.display = 'flex';
        scanOverlay.style.display = 'none';
    });
}