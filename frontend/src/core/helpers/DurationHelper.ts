class DurationHelper {
  public static callDurationColorDecider = (duration: number) => {
    if (duration < 60) {
      return "success";
    } else if (duration < 180) {
      return "warning";
    } else {
      return "danger";
    }
  };

  public static callAwaitDurationColorDecider = (duration: number) => {
    if (duration < 20) {
      return "success";
    } else if (duration < 40) {
      return "warning";
    } else {
      return "danger";
    }
  };
}

export default DurationHelper;
