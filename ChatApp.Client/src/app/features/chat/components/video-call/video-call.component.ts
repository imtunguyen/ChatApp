import { AfterViewInit, Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { WebrtcService } from '../../../../core/services/webrtc.service';
import { SignalRService } from '../../../../core/services/signalr.service';

@Component({
  selector: 'app-video-call',
  imports: [],
  templateUrl: './video-call.component.html',
  styleUrl: './video-call.component.scss'
})
export class VideoCallComponent implements AfterViewInit {
  @ViewChild('localVideo') localVideo!: ElementRef;
  @ViewChild('remoteVideo') remoteVideo!: ElementRef;
  localStream!: MediaStream;
  remoteStream!: MediaStream;

  async ngAfterViewInit() {
    try {
      this.localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
      this.localVideo.nativeElement.srcObject = this.localStream;
      console.log("✅ Camera đã hoạt động và hiển thị video.");
    } catch (error) {
      console.error("❌ Lỗi mở webcam:", error);
    }
  }

  startCall() {
    // Giả lập remote stream (dành cho test)
    this.remoteStream = new MediaStream(this.localStream.getTracks()); 
    this.remoteVideo.nativeElement.srcObject = this.remoteStream;
    console.log("📞 Cuộc gọi bắt đầu!");
  }

  endCall() {
    this.localStream.getTracks().forEach(track => track.stop());
    this.remoteStream?.getTracks().forEach(track => track.stop());
    console.log("❌ Đã kết thúc cuộc gọi!");
  }
}
