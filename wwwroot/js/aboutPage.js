// JQuery comes as part of boostrap...
$("#cardTextOne").hide(0);
$("#cardTextTwo").hide(0);
$("#cardTextThree").hide(0);
$(document).ready(function () {
  let marker = 1;
  // jQuery methods go here...

  $("#cardTextOne").delay(1000).fadeIn(1000);

  $("#aboutCards").on("click", (event) => {
    if (!event.detail || event.detail == 1) {
      if (marker === 1) {
        $("#cardTextOne").fadeOut(500);
        $("#cardTextTwo").delay(500).fadeIn(500);
        marker += 1;
        return; //end the loop
      }

      if (marker === 2) {
        $("#cardTextTwo").fadeOut(500);
        $("#cardTextThree").delay(500).fadeIn(500);
        marker += 1;
        return;
      }

      if (marker === 3) {
        $("#cardTextThree").fadeOut(500);
        $("#cardTextOne").delay(500).fadeIn(500);
        marker = 1;
        return;
      }
    }
  });
});
