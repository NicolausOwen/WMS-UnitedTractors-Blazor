let codeReader;
let dotNetHelper;

window.scanner = {
    init: function (helper) {
        dotNetHelper = helper;
        if (!codeReader) {
            codeReader = new ZXing.BrowserMultiFormatReader();
        }
    },
    start: async function (videoId) {
        try {
            const devices = await codeReader.listVideoInputDevices();
            let selectedDeviceId = undefined;
            if (devices && devices.length > 0) {
                selectedDeviceId = devices[0].deviceId;
                const backCamera = devices.find(d => /back|rear|environment/i.test(d.label));
                if (backCamera) selectedDeviceId = backCamera.deviceId;
            }
            codeReader.decodeFromVideoDevice(selectedDeviceId, videoId, (result, err) => {
                if (result) {
                    playBeep();
                    dotNetHelper.invokeMethodAsync('OnBarcodeScanned', result.getText());
                    // stop reading after a successful scan
                    codeReader.reset();
                }
                if (err && !(err instanceof ZXing.NotFoundException)) {
                    console.error(err);
                }
            });
        } catch (error) {
            console.error("Camera access error:", error);
            alert("Tidak dapat mengakses kamera. Pastikan izin kamera sudah diberikan.");
            dotNetHelper.invokeMethodAsync('OnCameraError', error.message);
        }
    },
    stop: function () {
        if (codeReader) {
            codeReader.reset();
        }
    }
};

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
