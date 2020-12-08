const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);
const projectId = urlParams.get("id");
const fr = new FileReader();
var currentOcr;
Vue.component("recyclescroller", VueVirtualScroller.RecycleScroller);
var app = new Vue({
  el: "#app",
  data() {
    return {
      info: null,
      list: [],
      currentOcr: null,
      imageWidth: 0,
      imageHeight: 0,
      imageExtent: [],
      imageUri: null
    };
  },
  mounted() {
    axios.get(url, { params: { id: projectId } }).then(response => {
      this.list = response.data;
      console.log(response.data);
    });
  },
  methods: {
    select(event) {
      var targetId = event.currentTarget.id;
      this.getOcrJson(targetId).then(result => {
        this.currentOcr = result;
        this.setImage();
        console.log(this.getDataUrl("/Media/" + targetId));
      });
    },
    getOcrJson(targetId) {
      return $.getJSON("/Media/" + targetId + ".ocr.json");
    },
    getDataUrl(imgUrl) {
      console.log(imgUrl);
      // Create canvas
      const canvas = document.createElement("canvas");
      const ctx = canvas.getContext("2d");
      // Set width and height
      var img = new Image();
      img.onload = function() {
        ctx.drawImage(img, 0, 0);
      };
      img.src = imgUrl;
      return canvas.toDataURL("image/jpg");
    },
    setImage: function() {
      this.imageWidth = this.currentOcr.analyzeResult.readResults[0].width;
      this.imageHeight = this.currentOcr.analyzeResult.readResults[0].height;
      this.imageExtent = [0, 0, this.imageWidth, this.imageHeight];
      const projection = this.createProjection(this.imageExtent);
      console.log(projection);
    },
    createProjection(imageExtend) {
      return new ol.proj.Projection({
        code: "xkcd-image",
        units: "pixels",
        extent: imageExtend
      });
    },
    createImageSource(imageUri, projection, imageExtend) {
      return new ol.source.ImageStatic({
        url: imageUri,
        projection,
        imageExtent: imageExtend
      });
    },
    async loadImageToCanvas(imageUrl) {
      return new Promise((resolve, reject) => {
        const img = document.createElement("img");
        img.onload = async () => {
          const rotatedImage = renderRotatedImageToCanvas(img, 0);
          resolve(rotatedImage);
        };
        img.crossOrigin = "anonymous";
        img.onerror = reject;
        img.src = imageUrl;
      });
    }
  }
});
